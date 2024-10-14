/************************************************************
 * File:        INotificationService.cs
 * Author:      IT21252754 - Madhumalka K.C.S
 * Date:        2024-09-28
 * Description: This file defines the interface INotificationService, 
 *              which provides methods for creating and retrieving 
 *              notifications. The interface defines contract methods 
 *              that should be implemented by any notification service.
 ************************************************************/

using WebService.Models;

namespace WebService.Interfaces
{
    /// Interface that defines operations for managing notifications.
    public interface INotificationService
    {
        // Creates a new notification.
        Task CreateNotification(Notification notification);

        /// Retrieves a list of notifications by the receiver's ID.
        Task<List<Notification>> GetNotificationsByReceiverId(string receiverId);

        /// Retrieves a list of notifications by the admin role.
        Task<List<Notification>> GetNotificationsByAdminAndCSR(string role);

    }
}
