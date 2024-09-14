using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebService.Interfaces;
using WebService.Models;
using System;


namespace WebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly string _jwtSecret = "M2Y2YTc4ZGE0ZGI5ZDE1NjM5ZGVkMzk2MWE5NmU3YmFiOWEyOTkwM2M5NzQzZmUzZWQ4ZjllMzZjOGUyM2M="; // Should be in configuration

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User newUser)
        {
            await _userService.RegisterUser(newUser);
            return CreatedAtAction(nameof(Login), new { email = newUser.Email }, newUser);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var user = await _userService.AuthenticateUser(Email, Password);
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Role, user.Role) // Single role
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "your_app",
                audience: "your_app",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
