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
                return user;
            }
            return null;
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
                user.IsActive = !user.IsActive;
                await _userCollection.ReplaceOneAsync(x => x.Id == id, user);
            }
        }
    }
}
