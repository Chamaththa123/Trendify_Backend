/************************************************************
 * File:        Notification.cs
 * Author:      IT21252754 - Madhumalka K.C.S
 * Date:        2024-09-28
 * Description: This file defines the Notification model class, 
 *              which represents a notification stored in the 
 *              MongoDB database. It includes properties such 
 *              as receiver ID, message, and visibility flags.
 ************************************************************/

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace WebService.Models
{
    /// Represents a notification entity stored in MongoDB.
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
