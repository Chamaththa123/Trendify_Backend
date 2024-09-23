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

        public UserService(IOptions<MongoDBSettings> mongoDBSettings, IMongoClient mongoClient)
        {
            // Initialize the MongoDB collection
            var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _userCollection = mongoDatabase.GetCollection<User>(CollectionName);
        }


        public async Task<User> RegisterUser(User newUser)
        {
            newUser.PasswordHash = HashPassword(newUser.PasswordHash);
            await _userCollection.InsertOneAsync(newUser);
            return newUser;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _userCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

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

        private bool VerifyPassword(string password, string storedHash)
        {
            // Verify the hashed password
            var hashedPassword = HashPassword(password);
            return hashedPassword == storedHash;
        }

        //get all users
        public async Task<List<User>> GetAllUsers(string role)
        {
            return await _userCollection.Find(x => x.Role == role).ToListAsync();
        }

        //get specific product list
        public async Task<User?> GetUserById(string id)
        {
            return await _userCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        //change user status
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


        // Get vendors (role = 3)
        public async Task<List<User>> GetVendors()
        {
            return await _userCollection.Find(x => x.Role == "3").ToListAsync();
        }

        // Get CSRs (role = 2)
        public async Task<List<User>> GetCSRs()
        {
            return await _userCollection.Find(x => x.Role == "2").ToListAsync();
        }

        public async Task<List<User>> GetCustomers()
        {
            return await _userCollection.Find(x => x.Role == "0" && x.IsActive != 0).ToListAsync();
        }

        public async Task<List<User>> GetPendingCustomers()
        {
            return await _userCollection.Find(x => x.Role == "0" && x.IsActive == 0).ToListAsync();
        }

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

        public async Task<bool> UpdateUserDetails(string id, User updatedUser)
        {
            var user = await _userCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return false; // User not found
            }

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
