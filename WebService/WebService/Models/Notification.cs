using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace WebService.Models
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonRepresentation(BsonType.ObjectId)]
        public string? ReceiverId { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        public string? Message { get; set; }

        public bool IsVisibleToAdmin { get; set; } = false;
        public bool IsVisibleToCSR { get; set; } = false;
        public bool IsVisibleTovendor { get; set; } = false;


        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
