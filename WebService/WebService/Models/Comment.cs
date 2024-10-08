/************************************************************
 * File:        Comment.cs
 * Author:      IT21210174 - Tharushi Lakshika V.G
 * Date:        2024-09-22
 * Description: This file defines the Comment model which represents 
 *              comments made by customers for vendors. Each comment 
 *              includes customer and vendor IDs, comment content, 
 *              creation and update timestamps, and optional customer name.
 ************************************************************/

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WebService.Models
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonRepresentation(BsonType.ObjectId)]
        public string CustomerId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string VendorId { get; set; }

        [BsonRequired]
        public string comment { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? UpdatedAt { get; set; }

        [BsonIgnore]
        public string? CustomerName { get; set; }
    }
}
