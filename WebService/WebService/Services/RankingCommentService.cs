using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebService.Interfaces;
using WebService.Models;
using WebService.Settings;

namespace WebService.Services
{
    public class RankingCommentService : IRankingComment
    {
        // Initializes a new instance of the ProductService class.
        private readonly IMongoCollection<Ranking> _rankingCollection;
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<Comment> _commentCollection;

        // define mongodb collection name
        private const string RankingCollectionName = "ranking";
        private const string UserCollectionName = "user";
        private const string CommentCollectionName = "comment";

        public RankingCommentService(IOptions<MongoDBSettings> mongoDBSettings, IMongoClient mongoClient)
        {
            // Initialize the MongoDB collection
            var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _rankingCollection = mongoDatabase.GetCollection<Ranking>(RankingCollectionName);
            _userCollection = mongoDatabase.GetCollection<User>(UserCollectionName);
            _commentCollection = mongoDatabase.GetCollection<Comment>(CommentCollectionName);
        }

        // Creates a new ranking
        public async Task AddRankForVendor(Ranking ranking)
        {
            await _rankingCollection.InsertOneAsync(ranking);
            await UpdateVendorAverageRating(ranking.VendorId);
        }

        //get all rankings for vendor
        public async Task<List<Ranking>> GetRankingForVendor(string id)
        {
            return await _rankingCollection.Find(x => x.VendorId == id).ToListAsync();
        }

        private async Task UpdateVendorAverageRating(string vendorId)
        {
            // Get all rankings for the specific vendor
            var vendorRankings = await _rankingCollection.Find(x => x.VendorId == vendorId).ToListAsync();

            // Calculate the new average rating
            double averageRating = vendorRankings.Average(r => r.ranking);

            // Update the vendor's average rating in the vendor collection
            var update = Builders<User>.Update.Set(v => v.AverageRating, averageRating);
            await _userCollection.UpdateOneAsync(v => v.Id == vendorId, update);
        }

        public async Task AddCommentForVendor(Comment comment)
        {
            await _commentCollection.InsertOneAsync(comment);
        }

        public async Task EditCommentForVendor(string commentId, string customerId, string comment)
        {
            var filter = Builders<Comment>.Filter.Where(c => c.Id == commentId && c.CustomerId == customerId);
            var update = Builders<Comment>.Update
                .Set(c => c.comment, comment)
                .Set(c => c.UpdatedAt, DateTime.Now);

            var result = await _commentCollection.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
            {
                throw new InvalidOperationException("you do not have permission to edit this comment.");
            }
        }

        public async Task<List<Comment>> GetCommentsByVendorId(string vendorId)
        {
            var filter = Builders<Comment>.Filter.Eq(c => c.VendorId, vendorId);
            return await _commentCollection.Find(filter).ToListAsync();
        }
    }
}
