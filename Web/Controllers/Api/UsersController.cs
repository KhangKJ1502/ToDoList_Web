using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebUI.Controllers.Api
{
    [ApiController]
    [Route("api/")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid) {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Any())
                    .SelectMany(x => x.Value.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();
                return BadRequest(new { success = false, message = errors });
            }

            try {
                await _userService.RegisterUserAsync(userDto);
                return Ok(new { success = true, message = "User registered successfully." });
            } catch (System.InvalidOperationException ex) {
                return BadRequest(new { success = false, message = ex.Message });
            } catch (System.Exception ex) {
                return StatusCode(500, new { success = false, message = "An error occurred while registering the user." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid) {
                return BadRequest(new { success = false, message = "Invalid login data" });
            }

            try {
                var loginSuccessful = await _userService.LoginUserAsync(loginDTO.Email, loginDTO.Password);

                if (!loginSuccessful) {
                    return BadRequest(new { success = false, message = "Email or Password is incorrect" });
                }

                // Get user information for claims
                var user = await _userService.GetUserByEmailAsync(loginDTO.Email);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("UserId", user.UserId.ToString())
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", principal);

                return Ok(new { success = true, message = "User logged in successfully" });
            } catch (Exception ex) {
                // Log the exception
                Console.WriteLine($"Login error: {ex.Message}");
                return StatusCode(500, new { success = false, message = "An error occurred during login" });
            }
        }
    }
}