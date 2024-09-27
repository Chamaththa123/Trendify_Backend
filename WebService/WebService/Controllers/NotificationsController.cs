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
