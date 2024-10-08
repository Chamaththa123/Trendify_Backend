/************************************************************
 * File:        Ranking.cs
 * Author:      IT21210174 - Tharushi Lakshika V.G
 * Date:        2024-09-22
 * Description: This file defines the Ranking model representing 
 *              customer rankings for vendors. Each ranking includes
 *              customer and vendor IDs, the rank value, creation 
 *              timestamp, and optional customer name.
 ************************************************************/

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WebService.Models
{
    public class Ranking
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonRepresentation(BsonType.ObjectId)]
        public string CustomerId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string VendorId { get; set; }

        public int ranking { get; set; }

        [BsonIgnore]
        public string? CustomerName { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
