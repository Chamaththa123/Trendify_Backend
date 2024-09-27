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

        public NotificationService(IOptions<MongoDBSettings> mongoDBSettings, IMongoClient mongoClient)
        {
            var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _notificationCollection = mongoDatabase.GetCollection<Notification>("notifications");
        }

        public async Task CreateNotification(Notification notification)
        {
            await _notificationCollection.InsertOneAsync(notification);
        }

        public async Task<List<Notification>> GetNotificationsByReceiverId(string receiverId)
        {
            return await _notificationCollection.Find(n => n.ReceiverId == receiverId).ToListAsync();
        }
    }
}
