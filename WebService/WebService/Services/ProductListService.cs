/************************************************************
 * File:        ProductListService.cs
 * Author:      IT21210174 - Tharushi Lakshika V.G
 * Date:        2024-09-17
 * Description: This file implements the ProductListService class, 
 *              which provides CRUD operations for managing product 
 *              lists in MongoDB. It interacts with the Product_List 
 *              collection and supports operations like creation, 
 *              retrieval, update, deletion, and status change.
 ************************************************************/

using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebService.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using WebService.Settings;
using WebService.Models;

namespace WebService.Services
{
    // Provides implementation for the IProductListService interface.

    public class ProductListService : IProductListService
    {
        // Initializes a new instance of the ProductListService class.
        private readonly IMongoCollection<Product_List> _productListCollection;

        // define mongodb collection name
        private const string CollectionName = "product_list";

        public ProductListService(IOptions<MongoDBSettings> mongoDBSettings, IMongoClient mongoClient)
        {
            // Initialize the MongoDB collection
            var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _productListCollection = mongoDatabase.GetCollection<Product_List>(CollectionName);
        }

        // Creates a new  product list
        public async Task CreateProductList(Product_List newProductList)
        {
            await _productListCollection.InsertOneAsync(newProductList);
        }

        //get all product lists
        public async Task<List<Product_List>> GetProductList()
        {
            return await _productListCollection.Find(_ => true).ToListAsync();
        }

        //get all product lists
        public async Task<List<Product_List>> GetActiveProductList()
        {
            return await _productListCollection.Find(x => x.IsActive == true).ToListAsync();
        }

        //get specific product list
        public async Task<Product_List?> GetProductListById(string id)
        {
            return await _productListCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        //update product list
        public async Task UpdateProductList(string id, Product_List updatedProductList)
        {
            await _productListCollection.ReplaceOneAsync(x => x.Id == id, updatedProductList);
        }

        //delete product list
        public async Task RemoveProductList(string id)
        {
            await _productListCollection.DeleteOneAsync(x => x.Id == id);
        }

        //change product list status
        public async Task ChangeProductListStatus(string id)
        {
            var product = await _productListCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (product != null)
            {
                product.IsActive = !product.IsActive;
                await _productListCollection.ReplaceOneAsync(x => x.Id == id, product);
            }
        }
    }
}
