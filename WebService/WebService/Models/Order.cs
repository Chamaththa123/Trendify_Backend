using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace WebService.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public string CustomerId { get; set; } 

        [Required]
        public DateTime Date { get; set; } 

        [Required]
        public decimal TotalPrice { get; set; } 

        [Required]
        public int Status { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();  // List of ordered products

        public int OrderItemCount { get; set; }

        // Fields for cancellation requests
        public bool IsCancellationRequested { get; set; } = false;
        public string CancellationNote { get; set; }
        public int IsCancellationApproved { get; set; } = 0;

        [Required]
        public string Recipient_Name        { get; set; }

        [Required]
        public string Recipient_Email { get; set; }

        [Required]
        public string Recipient_Contact { get; set; }

        [Required]
        public string Recipient_Address { get; set; }

        [BsonIgnore]
        public string? CustomerFirstName { get; set; }
        [BsonIgnore]
        public string? CustomerLastName { get; set; }
    }

    public class OrderItem
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public string ProductId { get; set; }

        [BsonIgnore]
        public string? ProductName { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string VendorId { get; set; }

        [Required]
        public decimal UnitPrice { get; set; } 

        [Required]
        public int Quantity { get; set; } 

        [Required]
        public decimal Total { get; set; }

        [Required]
        public bool IsDelivered { get; set; } = false;

    }
}
