/************************************************************
 * File:        Product.cs
 * Author:      IT21252754 Madhumalka K.C.S
 * Date:        2024-09-25
 * Description: Represents a product in the system, including
 *              its properties, validation rules, and MongoDB
 *              serialization attributes.
 ************************************************************/

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace WebService.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [Required(ErrorMessage = "Name is required.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Product List is required.")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Product_idProductList { get; set; }

        [Required(ErrorMessage = "vendor is required.")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Product_idVendor { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Low stock level is required.")]
        public int LowStockLvl { get; set; }

        [Required(ErrorMessage = "Image is required.")]
        public string Image { get; set; }

        public bool IsActive { get; set; } = true;

        // Timestamp for when the product was created
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Timestamp for the last time the product was updated
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Property to hold the Product List Name
        [BsonIgnore]
        public string? ProductListName { get; set; }

        [BsonIgnore]
        public string? ProductVendorName { get; set; }

        [BsonIgnore]
        public string StockStatus => Stock == 0
           ? "Out of Stock"
           : Stock <= LowStockLvl
               ? "Low Stock"
               : "In Stock";
    }
}
