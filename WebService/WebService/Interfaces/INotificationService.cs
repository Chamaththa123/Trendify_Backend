using WebService.Models;

namespace WebService.Interfaces
{
    public interface INotificationService
    {
        Task CreateNotification(Notification notification);
        Task<List<Notification>> GetNotificationsByReceiverId(string receiverId);
    }
}
