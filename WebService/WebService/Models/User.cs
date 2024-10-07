/************************************************************
 * File:        User.cs
 * Author:      IT21252754 - Madhumalka K.C.S
 * Date:        2024-09-17
 * Description: This file defines the User class, which represents
 *              the user model for the web service. The class includes 
 *              properties for user information and applies MongoDB 
 *              and data validation attributes.
 ************************************************************/

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

        public double? AverageRating { get; set; }

        [Required]
        public int IsActive { get; set; } = 0;
    }
}
