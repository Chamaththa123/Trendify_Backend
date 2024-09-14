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

        [HttpGet]
        public async Task<List<Product>> GetProduct() =>
            await _productService.GetProduct();

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await _productService.GetProductById(id);

            if (product is null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Product newProduct)
        {
            await _productService.CreateProduct(newProduct);
            return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
        }

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var product = await _productService.GetProductById(id);

            if (product is null)
            {
                return NotFound();
            }

            await _productService.RemoveProduct(id);

            return NoContent();
        }

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
    }
}
