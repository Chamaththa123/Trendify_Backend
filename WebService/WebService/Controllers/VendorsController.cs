using Microsoft.AspNetCore.Mvc;
using WebService.Dto;
using WebService.Interfaces;
using WebService.Models;
using WebService.Services;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorsController : ControllerBase
    {
        private readonly IVendorService _vendorService;
        private readonly IUserService _userService;

        public VendorsController(IVendorService vendorService, IUserService userService)
        {
            _vendorService = vendorService;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterVendor([FromBody] VendorRegistrationDto registrationDto)
        {
            try
            {
                if (registrationDto == null)
                {
                    return BadRequest(new { message = "Invalid registration data." });
                }

                var newUser = registrationDto.User;
                var vendor = registrationDto.Vendor;

                if (newUser == null || vendor == null)
                {
                    return BadRequest(new { message = "User and Vendor details are required." });
                }

                // Create user
                var createdUser = await _userService.RegisterUser(newUser);

               
                //Console.WriteLine($"Created User ID: {createdUser.Id}");
                // Set the UserId in the Vendor model
                 vendor.UserId = createdUser.Id;
                 Console.WriteLine(vendor.UserId);

                System.Diagnostics.Debug.WriteLine("Debugging value: " + vendor.UserId);

                // Register the vendor with the updated UserId
                await _vendorService.RegisterVendor(vendor);

                return Ok(new { message = "Vendor registered successfully.", createdUser.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
