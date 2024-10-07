/************************************************************
 * File:        ProductListsController.cs
 * Author:      IT21210174 - Tharushi Lakshika V.G
 * Date:        2024-09-17
 * Description: This file contains the ProductListsController, 
 *              which provides API endpoints to manage product 
 *              lists. It supports creating, retrieving, updating, 
 *              deleting, and changing the status of product lists.
 ************************************************************/

using Microsoft.AspNetCore.Mvc;
using WebService.Interfaces;
using WebService.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WebService.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProductListsController : ControllerBase
    {
        private readonly IProductListService _productListService;

        public ProductListsController(IProductListService productListService) =>
            _productListService = productListService;

        [HttpGet]
        public async Task<List<Product_List>> GetProductList() =>
            await _productListService.GetProductList();

        [HttpGet("active")]
        public async Task<List<Product_List>> GetActiveProductList() =>
            await _productListService.GetActiveProductList();

        [HttpGet("{id}")]
        public async Task<ActionResult<Product_List>> GetProductListById(string id)
        {
            var product = await _productListService.GetProductListById(id);

            if (product is null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Product_List newProductList)
        {
            await _productListService.CreateProductList(newProductList);
            return CreatedAtAction(nameof(GetProductListById), new { id = newProductList.Id }, newProductList);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductList(string id, Product_List updatedProductList)
        {
            var product = await _productListService.GetProductListById(id);

            if (product is null)
            {
                return NotFound();
            }

            updatedProductList.Id = product.Id;

            await _productListService.UpdateProductList(id, updatedProductList);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var product = await _productListService.GetProductListById(id);

            if (product is null)
            {
                return NotFound();
            }

            await _productListService.RemoveProductList(id);

            return NoContent();
        }

        [HttpPatch("{id}/state")]
        public async Task<IActionResult> ChangeState(string id)
        {
            var productList = await _productListService.GetProductListById(id);

            if (productList is null)
            {
                return NotFound();
            }

            await _productListService.ChangeProductListStatus(id);

            return NoContent();
        }
    }
}
