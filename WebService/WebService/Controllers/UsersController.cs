using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebService.Interfaces;
using WebService.Models;
using System;
using WebService.Services;


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
            try
            {
                // Validate empty or null fields
                var emptyFields = new List<string>();

                if (newUser == null)
                {
                    return BadRequest("User data is required.");
                }

                if (string.IsNullOrEmpty(newUser.Email))
                {
                    emptyFields.Add(nameof(newUser.Email));
                }

                if (string.IsNullOrEmpty(newUser.PasswordHash))
                {
                    emptyFields.Add(nameof(newUser.PasswordHash));
                }

                if (string.IsNullOrEmpty(newUser.First_Name))
                {
                    emptyFields.Add(nameof(newUser.First_Name));
                }

                if (string.IsNullOrEmpty(newUser.Last_Name))
                {
                    emptyFields.Add(nameof(newUser.Last_Name));
                }

                // If there are any empty fields, return them in the response
                if (emptyFields.Any())
                {
                    return BadRequest(new { Message = "The following fields are required:", EmptyFields = emptyFields });
                }

                await _userService.RegisterUser(newUser);
                return CreatedAtAction(nameof(Login), new { email = newUser.Email,message="User is registered Succuessfully !!" }, newUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred during registration", Details = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
                {
                    return BadRequest("Email and password are required");
                }

                var user = await _userService.AuthenticateUser(loginRequest.Email, loginRequest.Password);
                if (user == null)
                {
                    return Unauthorized("Invalid email or password");
                }

                if (!user.IsActive)
                {
                    return Unauthorized("Your account is deactivated. Please contact support.");
                }

                var token = GenerateJwtToken(user);

                var UserDetails = new
                {
                    user.Id,
                    user.Username,
                    user.First_Name,
                    user.Last_Name,
                    user.Email,
                    user.NIC,
                    user.Address,
                    user.Role,
                    user.IsActive
                };

                return Ok(new { Token = token,user= UserDetails, Message = "user is logined successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred during login", Details = ex.Message });

            }
           
        }

        private string GenerateJwtToken(User user)
        {
            try
            {
                // Add user details as claims
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),  // Email as a subject
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),  // Unique Token ID
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.GivenName, user.First_Name),
                    new Claim(ClaimTypes.Surname, user.Last_Name),
                    new Claim(ClaimTypes.Role, user.Role),

                    // Custom claim types
                    new Claim("NIC", user.NIC),
                    new Claim("Address", user.Address),
                    new Claim("IsActive", user.IsActive.ToString())
        };

                // Create the key from the secret
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // Generate the token
                var token = new JwtSecurityToken(
                    issuer: "your_app",
                    audience: "your_app",
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),  // Set token expiration time
                    signingCredentials: creds);

                // Return the generated token
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                // Handle token generation errors
                throw new InvalidOperationException("Failed to generate JWT token.", ex);
            }
        }

        [HttpGet]
        public async Task<List<User>> GetAllUsers(string role)
        {
            try
            {
                var users = await _userService.GetAllUsers(role);

                return users;
            } catch (Exception ex)
            {
                throw new InvalidOperationException("Fail to get all users", ex);
            }
        }

        [HttpPut("status/{id}")]
        public async Task<IActionResult> ChangeState(string id)
        {
            try
            {
                var user = await _userService.GetUserById(id);

                if (user is null)
                {
                    return NotFound();
                }

                await _userService.ChangeUserStatus(id);

                return NoContent();
            }
            catch (Exception ex) {
                return StatusCode(500, new { Message = "An error occurred during changing user status", Details = ex.Message });
            }
        }

        // Get all vendors (role = 3)
        [HttpGet("vendors")]
        public async Task<IActionResult> GetVendors()
        {
            try
            {
                var vendors = await _userService.GetVendors();
                return Ok(vendors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching vendors", Details = ex.Message });
            }
        }

        // Get all CSRs (role = 2)
        [HttpGet("csrs")]
        public async Task<IActionResult> GetCSRs()
        {
            try
            {
                var csrs = await _userService.GetCSRs();
                return Ok(csrs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching CSRs", Details = ex.Message });
            }
        }
    }
}
