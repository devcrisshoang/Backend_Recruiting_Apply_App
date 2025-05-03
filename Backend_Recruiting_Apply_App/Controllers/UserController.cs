using Backend_Recruiting_Apply_App.Data.DTOs;
using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.Data.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemAPIdotnet.Data;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly RAADbContext _context;

        public UserController(RAADbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }

        [HttpGet("non-auth{id}")]
        public async Task<ActionResult<UserDTO>> GetNonAuthUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(UserMapper.ToDTO(user));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.ID }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.ID)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound(new { message = "User not found" });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPut("{id}/name")]
        public async Task<IActionResult> UpdateUserName(int id, [FromBody] string name)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            user.Name = name;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}/email")]
        public async Task<IActionResult> UpdateUserEmail(int id, [FromBody] string email)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            user.Email = email;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}/phone")]
        public async Task<IActionResult> UpdateUserPhone(int id, [FromBody] string phone)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            user.Phone = phone;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}/image")]
        public async Task<IActionResult> UpdateUserImage(int id, [FromBody] byte[] image)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            user.Image = image;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}/type")]
        public async Task<IActionResult> UpdateUserType(int id, [FromBody] int type)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            user.Type = type;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Controller sửa đổi: Lấy thông tin User dựa trên Applicant_ID, luôn trả về 200 OK
        [HttpGet("by-applicant/{applicantId}")]
        public async Task<ActionResult<object>> GetUserByApplicantId(int applicantId)
        {
            // Tìm Applicant dựa trên Applicant_ID
            var applicant = await _context.Applicant
                .FirstOrDefaultAsync(a => a.ID == applicantId);

            if (applicant == null)
            {
                return Ok();
            }

            // Tìm User dựa trên User_ID từ Applicant
            var user = await _context.User
                .FirstOrDefaultAsync(u => u.ID == applicant.User_ID);

            if (user == null)
            {
                return Ok();
            }

            // Chuyển User thành UserDTO để trả về
            return Ok(UserMapper.ToDTO(user));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.ID == id);
        }
    }
}