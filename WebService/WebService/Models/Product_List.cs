/************************************************************
 * File:        Product_List.cs
 * Author:      IT21252754 - Madhumalka K.C.S
 * Date:        2024-09-17
 * Description: This file defines the Product_List model used in 
 *              the WebService to represent product lists stored 
 *              in MongoDB. It includes properties for product 
 *              name, description, and active status.
 ************************************************************/



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
