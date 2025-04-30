using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebUI.Controllers
{
    [ApiController]
    [Route("api/")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService) // ✅ Correct interface
        {
            _userService = userService;
        } 

        [HttpGet("register-view")]
        public IActionResult RegisterAccount()
        {
            return View(); // Register.cshtml
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            if (userDto == null ||
                string.IsNullOrEmpty(userDto.Username) ||
                string.IsNullOrEmpty(userDto.Email) ||
                string.IsNullOrEmpty(userDto.Password)) {
                return BadRequest(new { success = false, message = "Invalid data." });
            }
            await _userService.RegisterUserAsync(userDto);

            // Normally you'd call your service here to save the user
            return Ok(new { success = true, message = "User registered successfully." });
        }

        [HttpGet("login-view")]
        public IActionResult LoginAccount()
        {
            return View("Login"); // Login.cshtml
        }
    }
}
