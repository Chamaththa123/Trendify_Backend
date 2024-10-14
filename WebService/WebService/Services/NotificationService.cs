/************************************************************
 * File:        NotificationService.cs
 * Author:      IT21252754 - Madhumalka K.C.S
 * Date:        2024-09-28
 * Description: This file implements the NotificationService class, 
 *              which provides methods for creating and retrieving 
 *              notifications from the MongoDB collection.
 ************************************************************/

using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebService.Interfaces;
using WebService.Models;
using WebService.Settings;

namespace WebService.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMongoCollection<Notification> _notificationCollection;

        /// Initializes a new instance of the NotificationService class.
        public NotificationService(IOptions<MongoDBSettings> mongoDBSettings, IMongoClient mongoClient)
        {
            var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _notificationCollection = mongoDatabase.GetCollection<Notification>("notifications");
        }

        /// Creates a new notification and stores it in the MongoDB collection.
        public async Task CreateNotification(Notification notification)
        {
            await _notificationCollection.InsertOneAsync(notification);
        }

        /// Retrieves all notifications for a specific receiver.
        public async Task<List<Notification>> GetNotificationsByReceiverId(string receiverId)
        {
            return await _notificationCollection.Find(n => n.ReceiverId == receiverId).ToListAsync();
        }

        /// Retrieves all notifications for admin.
        public async Task<List<Notification>> GetNotificationsByAdminAndCSR(string role)
        {
            if (role == "2")
            {
                return await _notificationCollection.Find(n => n.IsVisibleToCSR == true).ToListAsync();
            }
            else if (role == "1") 
            {

                return await _notificationCollection.Find(n => n.IsVisibleToAdmin == true).ToListAsync();
            }
            else
            {
                throw new ArgumentException("Invalid role provided. No notifications available.");
            }
        }
    }
}
