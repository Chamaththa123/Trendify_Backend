/************************************************************
 * File:        UsersController.cs
 * Author:      IT21252754 - Madhumalka K.C.S
 * Date:        2024-09-17
 * Description: UsersController is responsible for handling user-related operations such as registration, login,
    password management, and retrieving user details. It interacts with IUserService and INotificationService
    to perform the required actions.
 ************************************************************/

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
        private readonly INotificationService _notificationService;
        private readonly string _jwtSecret = "M2Y2YTc4ZGE0ZGI5ZDE1NjM5ZGVkMzk2MWE5NmU3YmFiOWEyOTkwM2M5NzQzZmUzZWQ4ZjllMzZjOGUyM2M="; // Should be in configuration

        /// Constructor for UsersController, initializes user and notification services.
        public UsersController(IUserService userService, INotificationService notificationService)
        {
            _userService = userService;
            _notificationService = notificationService;
        }

        /// Registers a new user. Sends notifications for customer role and initializes fields for specific roles.
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

                // Check if the email already exists
                var existingUser = await _userService.GetUserByEmail(newUser.Email);
                if (existingUser != null)
                {
                    return Conflict(new { Message = "Email is already in use." });
                }

                // Set Pending (0) only for role = 0
                if (newUser.Role == "0")
                {
                    newUser.IsActive = 0;

                    // Generate notification
                    var notificationMessage = $"New customer registration: {newUser.First_Name} {newUser.Last_Name} has registered with the email {newUser.Email}. Please review and approve the account.";
                    var notification = new Notification
                    {
                        IsVisibleToCSR = true,
                        IsVisibleToAdmin = true,
                        Message = notificationMessage
                    };

                    await _notificationService.CreateNotification(notification);
                }

                else
                {
                    newUser.IsActive = 1; 
                }

                // Set averageRating only for role = 3
                if (newUser.Role == "3")
                {
                    newUser.AverageRating = 0; // Initialize averageRating for role 3 users
                }
                else
                {
                    newUser.AverageRating = null; // Ensure averageRating is not set for other roles
                }

                await _userService.RegisterUser(newUser);

                if (newUser.Role == "3")
                {
                    newUser.AverageRating = 0; // Initialize averageRating for role 3 users
                }
                return CreatedAtAction(nameof(Login), new { email = newUser.Email, message = "User is registered successfully!" }, newUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred during registration", Details = ex.Message });
            }
        }

        /// Authenticates a user by their email and password, generates a JWT token if successful.
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

                // Check if the user role is either 1, 2, or 3
                if (user.Role != "1" && user.Role != "2" && user.Role != "3")
                {
                    return Unauthorized("Access denied. Only users with roles 1, 2, or 3 can log in.");
                }

                // Check IsActive status
                if (user.IsActive == 2)
                {
                    return Unauthorized("Your account is inactive. Please contact support.");
                }
                else if (user.IsActive == 0)
                {
                    return Unauthorized("Your account is pending approval.");
                }

                var token = GenerateJwtToken(user);

                var userDetails = new
                {
                    user.Id,
                    user.First_Name,
                    user.Last_Name,
                    user.Email,
                    user.NIC,
                    user.Address,
                    user.Role,
                    user.IsActive
                };

                return Ok(new { Token = token, user = userDetails, Message = "User logged in successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred during login", Details = ex.Message });
            }
        }

        /// Authenticates a customer by their email and password, generates a JWT token if successful.
        [HttpPost("customer-login")]
        public async Task<IActionResult> CustomerLogin([FromBody] LoginRequest loginRequest)
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

                // Check IsActive status
                if (user.IsActive == 2)
                {
                    return Unauthorized("Your account is inactive. Please contact support.");
                }
                else if (user.IsActive == 0)
                {
                    return Unauthorized("Your account is pending approval.");
                }

                var token = GenerateJwtToken(user);

                var userDetails = new
                {
                    user.Id,
                    user.First_Name,
                    user.Last_Name,
                    user.Email,
                    user.NIC,
                    user.Address,
                    user.Role,
                    user.IsActive
                };

                return Ok(new { Token = token, user = userDetails, Message = "Customer logged in successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred during customer login", Details = ex.Message });
            }
        }


        /// Generates a JWT token for the authenticated user.
        private string GenerateJwtToken(User user)
        {
            try
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.GivenName, user.First_Name),
                    new Claim(ClaimTypes.Surname, user.Last_Name),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("NIC", user.NIC),
                    new Claim("Address", user.Address),
                    new Claim("IsActive", user.IsActive.ToString())
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
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to generate JWT token.", ex);
            }
        }

        /// Retrieves a list of all users.
        [HttpGet]
        public async Task<List<User>> GetAllUsers(string role)
        {
            try
            {
                var users = await _userService.GetAllUsers(role);
                return users;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to get all users", ex);
            }
        }

        //change user status
        [HttpPut("status/{id}")]
        public async Task<IActionResult> ChangeState(string id)
        {
            try
            {
                var user = await _userService.GetUserById(id);

                if (user == null)
                {
                    return NotFound();
                }

                await _userService.ChangeUserStatus(id);
                return NoContent();
            }
            catch (Exception ex)
            {
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

        //get all customer list
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var customers = await _userService.GetCustomers();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching customers", Details = ex.Message });
            }
        }

        //get all customer list that status == pending
        [HttpGet("customers/pending")]
        public async Task<IActionResult> GetPendingCustomers()
        {
            try
            {
                var customers = await _userService.GetPendingCustomers();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching customers", Details = ex.Message });
            }
        }

        //user changes password
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(changePasswordRequest.Email) ||
                    string.IsNullOrEmpty(changePasswordRequest.CurrentPassword) ||
                    string.IsNullOrEmpty(changePasswordRequest.NewPassword))
                {
                    return BadRequest("Email, current password, and new password are required.");
                }

                // Check if the email exists in the system
                var user = await _userService.GetUserByEmail(changePasswordRequest.Email);
                if (user == null)
                {
                    return NotFound("User with this email does not exist.");
                }

                // Call the service to change the password
                var result = await _userService.ChangePassword(
                    changePasswordRequest.Email,
                    changePasswordRequest.CurrentPassword,
                    changePasswordRequest.NewPassword);

                if (result)
                {
                    return Ok(new { Message = "Password changed successfully." });
                }

                return StatusCode(500, "An error occurred while changing the password.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred during password change", Details = ex.Message });
            }
        }


        //update user details
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUserDetails(string id, [FromBody] User updatedUser)
        {
            try
            {
                // Validate the updated user object
                if (updatedUser == null || string.IsNullOrEmpty(id))
                {
                    return BadRequest("Invalid user data.");
                }

                // Check if the user exists
                var existingUser = await _userService.GetUserById(id);
                if (existingUser == null)
                {
                    return NotFound("User not found.");
                }

                // Check if email is changing and if it already exists in another user
                if (updatedUser.Email != existingUser.Email)
                {
                    var emailCheckUser = await _userService.GetUserByEmail(updatedUser.Email);
                    if (emailCheckUser != null)
                    {
                        return Conflict("Email is already in use by another account.");
                    }
                }

                // Call the service to update user details (excluding password)
                var result = await _userService.UpdateUserDetails(id, updatedUser);
                if (result)
                {
                    return Ok(new { Message = "User details updated successfully." });
                }

                return StatusCode(500, "An error occurred while updating the user details.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
            }
        }


        //get user details by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var user = await _userService.GetUserById(id);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found." });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching the user", Details = ex.Message });
            }
        }

    }
}
