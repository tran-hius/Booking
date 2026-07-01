using Booking.DTOs.User.Request;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateByAdmin([FromBody] CreateUserDTO request)
        {
            var result = await _userService.CreateUserByAdminAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDTO request)
        {
            var result = await _userService.UpdateUserAsync(id, request);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            await _userService.SoftDeleteUserAsync(id);
            return Ok(new { message = "Xóa mềm người dùng thành công." });
        }

        [HttpPut("{id:int}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            await _userService.RestoreUserAsync(id);
            return Ok(new { message = "Khôi phục tài khoản người dùng thành công." });
        }
    }
}