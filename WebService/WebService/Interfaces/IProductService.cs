/************************************************************
 * File:        IProductService.cs
 * Author:      IT21252754 Madhumalka K.C.S
 * Date:        2024-09-25
 * Description: Interface for managing products in the 
 *              application. Defines methods for performing 
 *              CRUD operations on products as well as 
 *              managing stock levels.
 ************************************************************/

using WebService.Models;

namespace WebService.Interfaces
{

    // This interface provides methods for managing products.

    public interface IProductService
    {
        //get all product 
        Task<List<Product>> GetProduct();

        //get all active product 
        Task<List<Product>> GetActiveProduct();

        //get specific product  by id
        Task<Product?> GetProductById(string id);

        //create new product 
        Task CreateProduct(Product newProduct);

        //update avilable product details
        Task UpdateProduct(string id, Product updatedProduct);

        //delete available product
        Task RemoveProduct(string id);

        //activate or deactivate product
        Task ChangeProductStatus(string id);

        // Update stock for a specific product by reducing a quantity
        Task ReduceStockById(string productId, int quantity);

        // Update stock by adding new  stock values
        Task UpdateStock(string id, int additionalStock);

        // Reset stock to zero
        Task ResetStock(string id);
    }
}
