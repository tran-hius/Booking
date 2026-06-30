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

        [HttpPost]
        public async Task<IActionResult> CreateByAdmin([FromBody] CreateUserDTO request)
        {
            try
            {
                var result = await _userService.CreateUserByAdminAsync(request);

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi lấy danh sách người dùng.", detail = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = $"Không tìm thấy người dùng có ID = {id}" });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                await _userService.SoftDeleteUserAsync(id);
                return Ok(new { message = "Xóa mềm người dùng thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                await _userService.RestoreUserAsync(id);
                return Ok(new { message = "Khôi phục tài khoản người dùng thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
