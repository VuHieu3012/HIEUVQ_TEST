using Microsoft.AspNetCore.Mvc;
using AuthModule.Models;
using AuthModule.DTOs;
using AuthModule.Services;

namespace AuthModule.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthResponse { Success = false, Message = "Invalid input data" });

            var result = await _authService.LoginAsync(loginModel);
            return result.Success ? Ok(result) : Unauthorized(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthResponse { Success = false, Message = "Invalid input data" });

            var result = await _authService.RegisterAsync(registerModel);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateToken([FromBody] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest(new AuthResponse { Success = false, Message = "Token is required" });

            var result = await _authService.ValidateTokenAsync(token);
            return result.Success ? Ok(result) : Unauthorized(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new AuthResponse { Success = false, Message = "Refresh token is required" });

            var result = await _authService.RefreshTokenAsync(refreshToken);
            return result.Success ? Ok(result) : Unauthorized(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string token)
        {
            var result = await _authService.LogoutAsync(token);
            return result 
                ? Ok(new { success = true, message = "Logout successful" })
                : BadRequest(new { success = false, message = "Logout failed" });
        }


    }
}
