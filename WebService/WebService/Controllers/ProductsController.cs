/************************************************************
 * File:        ProductsController.cs
 * Author:      IT21252754 Madhumalka K.C.S
 * Date:        2024-09-25
 * Description: ASP.NET Core controller for managing products. 
 *              Provides API endpoints to create, read, update, 
 *              and delete products as well as manage stock levels.
 ************************************************************/

using Microsoft.AspNetCore.Mvc;
using WebService.Interfaces;
using WebService.Models;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService) =>
            _productService = productService;

        /// Gets all products.
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProduct()
        {
            var products = await _productService.GetProduct();

            if (products == null || products.Count == 0)
            {
                return NotFound(new { Message = "No products found." });
            }

            return Ok(products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                p.Description,
                p.Stock,
                p.LowStockLvl,
                StockStatus = p.StockStatus, // Include stock status in response
                p.Image,
                p.IsActive,
                p.Product_idVendor,
                p.ProductVendorName,
                p.ProductListName
            }));
        }

        /// Gets all active products.
        [HttpGet("active")]
        public async Task<ActionResult<List<Product>>> GetActiveProduct()
        {
            var products = await _productService.GetActiveProduct();

            if (products == null || products.Count == 0)
            {
                return NotFound(new { Message = "No products found." });
            }

            return Ok(products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                p.Description,
                p.Stock,
                p.LowStockLvl,
                StockStatus = p.StockStatus, // Include stock status in response
                p.Image,
                p.IsActive,
                p.Product_idVendor,
                p.ProductVendorName,
                p.ProductListName
            }));
        }

        /// Gets all active products.
        [HttpGet("vendorId/{id}")]
        public async Task<ActionResult<List<Product>>> GetProductByVendorId(string id)
        {
            var products = await _productService.GetProductByVendorId(id);

            if (products == null || products.Count == 0)
            {
                return NotFound(new { Message = "No products found." });
            }

            return Ok(products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                p.Description,
                p.Stock,
                p.LowStockLvl,
                StockStatus = p.StockStatus, // Include stock status in response
                p.Image,
                p.IsActive,
                p.Product_idVendor,
                p.ProductVendorName,
                p.ProductListName
            }));
        }

        // Get product by ID with stock status
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await _productService.GetProductById(id);

            if (product is null)
            {
                return NotFound();
            }

            return Ok(new
            {
                product.Id,
                product.Name,
                product.Price,
                product.Description,
                product.Stock,
                product.LowStockLvl,
                StockStatus = product.StockStatus, // Include stock status in response
                product.Image,
                product.IsActive,
                product.Product_idVendor,
                product.ProductListName,
                product.ProductVendorName,
                product.Product_idProductList
            });
        }

        /// Creates a new product.
        [HttpPost]
        public async Task<IActionResult> Post(Product newProduct)
        {
            await _productService.CreateProduct(newProduct);
            return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
        }

        /// Updates an existing product by its ID.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id, Product updatedProduct)
        {
            var product = await _productService.GetProductById(id);

            if (product is null)
            {
                return NotFound();
            }

            updatedProduct.Id = product.Id;

            await _productService.UpdateProduct(id, updatedProduct);

            return NoContent();
        }

        /// Deletes a product by its ID.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var product = await _productService.GetProductById(id);

            if (product is null)
            {
                return NotFound();
            }

            try
            {
                await _productService.RemoveProduct(id);
                return Ok(new { Message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                // Check if the exception message is related to pending orders
                if (ex.Message.Contains("pending orders"))
                {
                    // Return 409 Conflict with a proper message
                    return Conflict(new { Message = ex.Message });
                }

                // For other exceptions, return a generic server error
                return StatusCode(500, new { Message = "An error occurred while deleting the product." });
            }
        }

        /// Changes the status of a product by its ID.
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(string id)
        {
            var product = await _productService.GetProductById(id);

            if (product is null)
            {
                return NotFound();
            }

            await _productService.ChangeProductStatus(id);

            return NoContent();
        }

        /// Updates the stock of a product based on the specified stock update information.
        [HttpPut("stocks/update/{id}")]
        public async Task<IActionResult> UpdateStock(string id, [FromBody] StockUpdate stockUpdate)
        {
            var product = await _productService.GetProductById(id);

            if (product is null)
            {
                return NotFound();
            }

            if (stockUpdate.Type == 1)
            {
                // Add stock
                await _productService.UpdateStock(id, stockUpdate.StockChange);
            }
            else if (stockUpdate.Type == 0)
            {
                // Reduce stock
                await _productService.UpdateStock(id, -stockUpdate.StockChange);
            }

            return NoContent();
        }

        /// Resets the stock of a product to its initial value.
        [HttpPut("stocks/reset/{id}")]
        public async Task<IActionResult> ResetStock(string id)
        {
            var product = await _productService.GetProductById(id);

            if (product is null)
            {
                return NotFound();
            }

            await _productService.ResetStock(id);
            return NoContent();
        }
    }
}
