using WebService.Models;

namespace WebService.Interfaces
{

    // This interface provides methods for managing products.

    public interface IProductService
    {
        //get all product 
        Task<List<Product>> GetProduct();

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

        // Update stock by adding new  stock values
        Task UpdateStock(string id, int additionalStock);

        // Reset stock to zero
        Task ResetStock(string id);
    }
}
