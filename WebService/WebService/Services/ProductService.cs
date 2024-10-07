using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebService.Interfaces;
using WebService.Models;
using WebService.Settings;

namespace WebService.Services
{
    // Provides implementation for the IProductService interface.

    public class ProductService : IProductService
    {
        // Initializes a new instance of the ProductService class.
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMongoCollection<Product_List> _productListCollection;
        private readonly IMongoCollection<User> _userCollection;
        private readonly INotificationService _notificationService;

        // define mongodb collection name
        private const string CollectionName = "product";

        public ProductService(IOptions<MongoDBSettings> mongoDBSettings, IMongoClient mongoClient, INotificationService notificationService)
        {
            // Initialize the MongoDB collection
            var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _productCollection = mongoDatabase.GetCollection<Product>(CollectionName);
            _productListCollection = mongoDatabase.GetCollection<Product_List>("product_list");
            _userCollection = mongoDatabase.GetCollection<User>("user");

            _notificationService = notificationService;
        }

        // Creates a new  product 
        public async Task CreateProduct(Product newProduct)
        {
            var productList = await _productListCollection.Find(x => x.Id == newProduct.Product_idProductList).FirstOrDefaultAsync();
            if (productList == null)
            {
                // Handle case where the product list ID does not exist
                throw new Exception("Invalid Product List ID");
            }

            await _productCollection.InsertOneAsync(newProduct);
        }

        //get all product 
        public async Task<List<Product>> GetProduct()
        {
            // Fetch all products
            var products = await _productCollection.Find(_ => true).ToListAsync();

            // Fetch all product lists
            var productLists = await _productListCollection.Find(_ => true).ToListAsync();

            // Fetch all product lists
            var venodrs = await _userCollection.Find(_ => true).ToListAsync();

            // Join products with product lists to get product list names
            var productListDictionary = productLists.ToDictionary(pl => pl.Id);

            var vendorDictionary = venodrs.ToDictionary(pl => pl.Id);

            foreach (var product in products)
            {
                if (productListDictionary.TryGetValue(product.Product_idProductList ?? string.Empty, out var productList))
                {
                    product.ProductListName = productList.Name;
                }

                if (vendorDictionary.TryGetValue(product.Product_idVendor ?? string.Empty, out var user))
                {
                    product.ProductVendorName = user.First_Name + user.Last_Name;
                }
            }

            return products;
        }

        //get specific product
        public async Task<Product?> GetProductById(string id)
        {
            var product = await _productCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (product != null)
            {
                // Fetch the product list
                var productList = await _productListCollection.Find(x => x.Id == product.Product_idProductList).FirstOrDefaultAsync();
                if (productList != null)
                {
                    product.ProductListName = productList.Name;
                }
            }

            return product;
        }

        //update product 
        public async Task UpdateProduct(string id, Product updatedProduct)
        {
            await _productCollection.ReplaceOneAsync(x => x.Id == id, updatedProduct);
        }

        //delete product
        public async Task RemoveProduct(string id)
        {
            await _productCollection.DeleteOneAsync(x => x.Id == id);
        }

        //change product status
        public async Task ChangeProductStatus(string id)
        {
            var product = await _productCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (product != null)
            {
                product.IsActive = !product.IsActive;
                await _productCollection.ReplaceOneAsync(x => x.Id == id, product);
            }
        }

        // Update stock by adding new values
        public async Task UpdateStock(string id, int additionalStock)
        {
            var product = await _productCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            product.Stock += additionalStock;
            await _productCollection.ReplaceOneAsync(x => x.Id == id, product);

            // Check if stock is low
            if (product.Stock <= product.LowStockLvl)
            {
                // Generate notification
                var notification = new Notification
                {
                    ReceiverId = product.Product_idVendor, // Product's vendor is the receiver
                    Message = $"Product '{product.Name}' is running low on stock. Current stock: {product.Stock}"
                };

                await _notificationService.CreateNotification(notification);
            }
        }

        // Reset stock to zero
        public async Task ResetStock(string id)
        {
            var product = await _productCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            product.Stock = 0;
            await _productCollection.ReplaceOneAsync(x => x.Id == id, product);
        }
    }
}
