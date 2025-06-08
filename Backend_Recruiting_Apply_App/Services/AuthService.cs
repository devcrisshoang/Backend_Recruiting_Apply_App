using Backend_Recruiting_Apply_App.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SystemAPIdotnet.Data;
using System.Security.Cryptography;
using System;

namespace Backend_Recruiting_Apply_App.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request);
        Task<(bool Success, string Token, string Message)> LoginAsync(LoginRequest request);
    }

    public class AuthService : IAuthService
    {
        private readonly RAADbContext _context;
        private readonly IConfiguration _config;
        private readonly byte[] _aesKey;
        private readonly byte[] _aesIV;

        public AuthService(RAADbContext context, IConfiguration config)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _config = config ?? throw new ArgumentNullException(nameof(config));

            string aesKey = _config["Aes:Key"];
            string aesIV = _config["Aes:IV"];

            Console.WriteLine($"Aes:Key = {aesKey}");
            Console.WriteLine($"Aes:IV = {aesIV}");

            if (string.IsNullOrEmpty(aesKey))
                throw new InvalidOperationException("Aes:Key is missing in configuration.");
            if (string.IsNullOrEmpty(aesIV))
                throw new InvalidOperationException("Aes:IV is missing in configuration.");

            try
            {
                _aesKey = Convert.FromBase64String(aesKey);
                if (_aesKey.Length != 32)
                    throw new InvalidOperationException("Aes:Key must be 32 bytes (256 bits) after base64 decoding.");
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException($"Invalid base64 format for Aes:Key: {aesKey}", ex);
            }

            try
            {
                _aesIV = Convert.FromBase64String(aesIV);
                if (_aesIV.Length != 16)
                    throw new InvalidOperationException("Aes:IV must be 16 bytes (128 bits) after base64 decoding.");
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException($"Invalid base64 format for Aes:IV: {aesIV}", ex);
            }
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request)
        {
            string encryptedUsername = EncryptString(request.Username);
            if (await _context.User.AnyAsync(u => u.Username == encryptedUsername))
            {
                return (false, "Username already exists");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User
            {
                Name = EncryptString(request.Name),
                Email = EncryptString(request.Email),
                Phone = EncryptString(request.Phone),
                Username = encryptedUsername,
                Password = hashedPassword,
                Type = request.Type
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return (true, "User registered successfully");
        }

        public async Task<(bool Success, string Token, string Message)> LoginAsync(LoginRequest request)
        {
            string encryptedUsername = EncryptString(request.Username);
            var user = await _context.User.FirstOrDefaultAsync(u => u.Username == encryptedUsername);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return (false, null, "Invalid username or password");
            }

            var token = GenerateJwtToken(user);
            return (true, token, "Login successful");
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                    new Claim(ClaimTypes.Name, DecryptString(user.Username)),
                    new Claim(ClaimTypes.Email, DecryptString(user.Email)),
                    new Claim(ClaimTypes.GivenName, DecryptString(user.Name)),
                    new Claim(ClaimTypes.MobilePhone, DecryptString(user.Phone)),
                    new Claim("UserType", user.Type.ToString())
                }),
                Expires = DateTime.Now.AddDays(3),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string EncryptString(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using (Aes aes = Aes.Create())
            {
                aes.Key = _aesKey;
                aes.IV = _aesIV;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        private string DecryptString(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            byte[] buffer = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = _aesKey;
                aes.IV = _aesIV;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}