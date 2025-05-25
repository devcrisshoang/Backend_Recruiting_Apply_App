using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.Data.DTOs;
using Backend_Recruiting_Apply_App.Data.Mappers;
using SystemAPIdotnet.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public UserService(RAADbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.User.ToListAsync();
        }

        public async Task<UserDTO> GetNonAuthUserAsync(int id)
        {
            var user = await _context.User.FindAsync(id);
            return user != null ? UserMapper.ToDTO(user) : null;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.User.FindAsync(id);
        }

        public async Task<UserDTO> GetUserByApplicantIdAsync(int applicantId)
        {
            var applicant = await _context.Applicant
                .FirstOrDefaultAsync(a => a.ID == applicantId);

            if (applicant == null)
                return null;

            var user = await _context.User
                .FirstOrDefaultAsync(u => u.ID == applicant.User_ID);

            return user != null ? UserMapper.ToDTO(user) : null;
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
    }
}