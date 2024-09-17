using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebService.Interfaces;
using WebService.Models;
using WebService.Settings;

namespace WebService.Services
{
    public class VendorService : IVendorService
    {
        private readonly IMongoCollection<Vendor> _vendorCollection;

        public VendorService(IMongoClient mongoClient, IOptions<MongoDBSettings> mongoDBSettings)
        {
            var database = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _vendorCollection = database.GetCollection<Vendor>("vendor");
        }

        public async Task RegisterVendor(Vendor vendor)
        {
            await _vendorCollection.InsertOneAsync(vendor);
        }
    }
}
