using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WebService.Models
{
    // Defines product list stored in MongoDB.
    public class Product_List
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
