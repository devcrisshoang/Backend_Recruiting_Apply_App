using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.Data.DTOs;
using Backend_Recruiting_Apply_App.Data.Mappers;
using SystemAPIdotnet.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Backend_Recruiting_Apply_App.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<UserDTO> GetNonAuthUserAsync(int id);
        Task<User> GetUserByIdAsync(int id);
        Task<UserDTO> GetUserByApplicantIdAsync(int applicantId);
        Task<User> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(int id, User user);
        Task<bool> UpdateUserNameAsync(int id, string name);
        Task<bool> UpdateUserEmailAsync(int id, string email);
        Task<bool> UpdateUserPhoneAsync(int id, string phone);
        Task<bool> UpdateUserImageAsync(int id, byte[] image);
        Task<bool> UpdateUserTypeAsync(int id, int type);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> UserExistsAsync(int id);
    }

    public class UserService : IUserService
    {
        private readonly RAADbContext _context;
        private readonly byte[] _aesKey;
        private readonly byte[] _aesIV;

        public UserService(RAADbContext context, IConfiguration config)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            string aesKey = config["Aes:Key"];
            string aesIV = config["Aes:IV"];

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

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = await _context.User.ToListAsync();
            foreach (var user in users)
            {
                user.Name = DecryptString(user.Name);
                user.Email = DecryptString(user.Email);
                user.Phone = DecryptString(user.Phone);
                user.Username = DecryptString(user.Username);
            }
            return users;
        }

        public async Task<UserDTO> GetNonAuthUserAsync(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
                return null;

            user.Name = DecryptString(user.Name);
            user.Email = DecryptString(user.Email);
            user.Phone = DecryptString(user.Phone);
            user.Username = DecryptString(user.Username);

            return UserMapper.ToDTO(user);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                user.Name = DecryptString(user.Name);
                user.Email = DecryptString(user.Email);
                user.Phone = DecryptString(user.Phone);
                user.Username = DecryptString(user.Username);
            }
            return user;
        }

        public async Task<UserDTO> GetUserByApplicantIdAsync(int applicantId)
        {
            var applicant = await _context.Applicant
                .FirstOrDefaultAsync(a => a.ID == applicantId);

            if (applicant == null)
                return null;

            var user = await _context.User
                .FirstOrDefaultAsync(u => u.ID == applicant.User_ID);

            if (user != null)
            {
                user.Name = DecryptString(user.Name);
                user.Email = DecryptString(user.Email);
                user.Phone = DecryptString(user.Phone);
                user.Username = DecryptString(user.Username);
                return UserMapper.ToDTO(user);
            }

            return null;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateUserAsync(int id, User user)
        {
            if (id != user.ID)
                return false;

            _context.Entry(user).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserExistsAsync(id))
                    return false;
                throw;
            }
        }

        public async Task<bool> UpdateUserNameAsync(int id, string name)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
                return false;

            user.Name = name;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserEmailAsync(int id, string email)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
                return false;

            user.Email = email;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserPhoneAsync(int id, string phone)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
                return false;

            user.Phone = phone;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserImageAsync(int id, byte[] image)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
                return false;

            user.Image = image;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserTypeAsync(int id, int type)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
                return false;

            user.Type = type;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
                return false;

            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            return await _context.User.AnyAsync(e => e.ID == id);
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