/************************************************************
 * File:        NotificationsController.cs
 * Author:      IT21252754 - Madhumalka K.C.S
 * Date:        2024-09-28
 * Description: This file implements the NotificationsController class, 
 *              which defines endpoints for managing notifications. 
 *              The controller interacts with the INotificationService 
 *              to provide notification-related functionalities via 
 *              API endpoints.
 ************************************************************/

using Microsoft.AspNetCore.Mvc;
using WebService.Interfaces;
using WebService.Models;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// Retrieves all notifications for a specific receiver by their ID.
        [HttpGet("{receiverId}")]
        public async Task<ActionResult<List<Notification>>> GetNotificationsByReceiverId(string receiverId)
        {
            var notifications = await _notificationService.GetNotificationsByReceiverId(receiverId);
            if (notifications == null || notifications.Count == 0)
            {
                return NotFound(new { Message = "No notifications found for this receiver." });
            }

            return Ok(notifications);
        }
    }
}
