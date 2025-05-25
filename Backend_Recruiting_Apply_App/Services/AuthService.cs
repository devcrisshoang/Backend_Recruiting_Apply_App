using Backend_Recruiting_Apply_App.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SystemAPIdotnet.Data;

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

        public AuthService(RAADbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request)
        {
            if (await _context.User.AnyAsync(u => u.Username == request.Username))
            {
                return (false, "Username already exists");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Username = request.Username,
                Password = hashedPassword,
                Type = request.Type
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return (true, "User registered successfully");
        }

        public async Task<(bool Success, string Token, string Message)> LoginAsync(LoginRequest request)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Username == request.Username);
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

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserType", user.Type.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
