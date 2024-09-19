using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace WebService.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [Required(ErrorMessage = "User Name is required.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "First Name is required.")]
        public string First_Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required.")]
        public string Last_Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "NIC is required.")]
        public string NIC { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public double AverageRating { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}
