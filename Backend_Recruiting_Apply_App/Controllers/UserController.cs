using Microsoft.AspNetCore.Mvc;
using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.Data.DTOs;
using Backend_Recruiting_Apply_App.Services;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("non-auth{id}")]
        public async Task<ActionResult<UserDTO>> GetNonAuthUser(int id)
        {
            var userDto = await _userService.GetNonAuthUserAsync(id);
            if (userDto == null)
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(userDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(user);
        }

        [HttpGet("by-applicant/{applicantId}")]
        public async Task<ActionResult<object>> GetUserByApplicantId(int applicantId)
        {
            var userDto = await _userService.GetUserByApplicantIdAsync(applicantId);
            return Ok(userDto);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.ID }, createdUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.ID)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            var result = await _userService.UpdateUserAsync(id, user);
            if (!result)
            {
                return NotFound(new { message = "User not found" });
            }
            return NoContent();
        }

        [HttpPut("{id}/name")]
        public async Task<IActionResult> UpdateUserName(int id, [FromBody] string name)
        {
            var result = await _userService.UpdateUserNameAsync(id, name);
            if (!result)
            {
                return NotFound(new { message = "User not found" });
            }
            return NoContent();
        }

        [HttpPut("{id}/email")]
        public async Task<IActionResult> UpdateUserEmail(int id, [FromBody] string email)
        {
            var result = await _userService.UpdateUserEmailAsync(id, email);
            if (!result)
            {
                return NotFound(new { message = "User not found" });
            }
            return NoContent();
        }

        [HttpPut("{id}/phone")]
        public async Task<IActionResult> UpdateUserPhone(int id, [FromBody] string phone)
        {
            var result = await _userService.UpdateUserPhoneAsync(id, phone);
            if (!result)
            {
                return NotFound(new { message = "User not found" });
            }
            return NoContent();
        }

        [HttpPut("{id}/image")]
        public async Task<IActionResult> UpdateUserImage(int id, [FromBody] byte[] image)
        {
            var result = await _userService.UpdateUserImageAsync(id, image);
            if (!result)
            {
                return NotFound(new { message = "User not found" });
            }
            return NoContent();
        }

        [HttpPut("{id}/type")]
        public async Task<IActionResult> UpdateUserType(int id, [FromBody] int type)
        {
            var result = await _userService.UpdateUserTypeAsync(id, type);
            if (!result)
            {
                return NotFound(new { message = "User not found" });
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound(new { message = "User not found" });
            }
            return NoContent();
        }
    }
}