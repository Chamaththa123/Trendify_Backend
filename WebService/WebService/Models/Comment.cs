﻿using MongoDB.Bson.Serialization.Attributes;
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
    }
}
