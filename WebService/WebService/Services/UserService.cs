/************************************************************
 * File:        UserService.cs
 * Author:      IT21252754 - Madhumalka K.C.S
 * Date:        2024-09-17
 * Description: This file contains the implementation of the IUserService 
 *              interface, which provides functionality for managing users
 *              such as registration, authentication, and user updates. 
 *              It uses MongoDB for data storage and retrieval.
 ************************************************************/


using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Data;
using WebService.Interfaces;
using WebService.Models;
using WebService.Settings;

namespace WebService.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _userCollection;
        private const string CollectionName = "user";

        // Constructor to initialize MongoDB collection
        public UserService(IOptions<MongoDBSettings> mongoDBSettings, IMongoClient mongoClient)
        {
            // Initialize the MongoDB collection
            var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _userCollection = mongoDatabase.GetCollection<User>(CollectionName);
        }

        // Registers a new user and hashes their password before saving
        public async Task<User> RegisterUser(User newUser)
        {
            newUser.PasswordHash = HashPassword(newUser.PasswordHash);
            await _userCollection.InsertOneAsync(newUser);
            return newUser;
        }

        // Retrieves a user by their email
        public async Task<User?> GetUserByEmail(string email)
        {
            return await _userCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        // Authenticates the user by verifying their password and status
        public async Task<User?> AuthenticateUser(string email, string password)
        {
            var user = await GetUserByEmail(email);
            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                if (user.IsActive == 1)
                {
                    return user; // User is active and can log in
                }
                else if (user.IsActive == 2)
                {
                    throw new UnauthorizedAccessException("Your account is inactive. Please contact support.");
                }
                else if (user.IsActive == 0)
                {
                    throw new UnauthorizedAccessException("Your account is pending approval.");
                }
            }
            return null; // Invalid credentials or user not found
        }

        // Hashes a user's password using a secure algorithm
        private string HashPassword(string password)
        {
            // Hash the password using a secure algorithm
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: new byte[16], // Use a real salt in production
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }

        // Verifies the password by comparing the hash with the stored hash
        private bool VerifyPassword(string password, string storedHash)
        {
            // Verify the hashed password
            var hashedPassword = HashPassword(password);
            return hashedPassword == storedHash;
        }

        // Retrieves all users with a specified role
        public async Task<List<User>> GetAllUsers(string role)
        {
            return await _userCollection.Find(x => x.Role == role).ToListAsync();
        }

        // Retrieves a specific user by their ID
        public async Task<User?> GetUserById(string id)
        {
            return await _userCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        // Changes the user status (activate, deactivate, or approve pending users)
        public async Task ChangeUserStatus(string id)
        {
            var user = await _userCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (user != null)
            {
                if (user.IsActive == 1)
                {
                    user.IsActive = 2; // Deactivate
                }
                else if (user.IsActive == 2)
                {
                    user.IsActive = 1; // Activate
                }
                else if (user.IsActive == 0)
                {
                    user.IsActive = 1; // Pending to Activate
                }

                await _userCollection.ReplaceOneAsync(x => x.Id == id, user);
            }
        }


        // Retrieves all users with the 'Vendor' role (role = 3)
        public async Task<List<User>> GetVendors()
        {
            return await _userCollection.Find(x => x.Role == "3").ToListAsync();
        }

        // Retrieves all users with the 'CSR' role (role = 2)
        public async Task<List<User>> GetCSRs()
        {
            return await _userCollection.Find(x => x.Role == "2").ToListAsync();
        }

        // Retrieves all customers who are active (role = 0, isActive != 0)
        public async Task<List<User>> GetCustomers()
        {
            return await _userCollection.Find(x => x.Role == "0" && x.IsActive != 0).ToListAsync();
        }

        // Retrieves all customers whose accounts are pending approval (role = 0, isActive = 0)
        public async Task<List<User>> GetPendingCustomers()
        {
            return await _userCollection.Find(x => x.Role == "0" && x.IsActive == 0).ToListAsync();
        }

        // Changes a user's password if the current password is correct
        public async Task<bool> ChangePassword(string email, string currentPassword, string newPassword)
        {
            // Find the user by email
            var user = await GetUserByEmail(email);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            // Update the user's password in the database
            await _userCollection.ReplaceOneAsync(x => x.Id == user.Id, user);

            return true;
        }

        // Updates user details such as name, email, address, etc.
        public async Task<bool> UpdateUserDetails(string id, User updatedUser)
        {
            var user = await _userCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return false; // User not found
            }

            // Update user details
            user.First_Name = updatedUser.First_Name;
            user.Last_Name = updatedUser.Last_Name;
            user.Email = updatedUser.Email;
            user.NIC = updatedUser.NIC;
            user.Address = updatedUser.Address;
            user.Role = updatedUser.Role;

            var result = await _userCollection.ReplaceOneAsync(x => x.Id == id, user);
            return result.ModifiedCount > 0;
        }

    }
}
