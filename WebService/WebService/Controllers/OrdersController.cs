using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebService.Interfaces;
using WebService.Models;
using WebService.Services;

namespace WebService.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        public OrdersController(IOrderService orderService, IProductService productService)
        {
            _orderService = orderService;
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order newOrder)
        {
            try
            {
                foreach (var item in newOrder.OrderItems)
                {
                    var id = item.ProductId;
                    // Fetch the product to get its price (if required)
                    var product = await _productService.GetProductById(id);

                    if (product == null)
                    {
                        throw new Exception($"Product with ID {item.ProductId} does not exist.");
                    }

                    // Set the total price for each item
                    item.Total = item.UnitPrice * item.Quantity;

                }

                // Calculate the total price of the order
                newOrder.TotalPrice = newOrder.OrderItems.Sum(i => i.Total);
                newOrder.Date = DateTime.UtcNow;

                newOrder.OrderItemCount = newOrder.OrderItems.Count;

                newOrder.Status = 0;

                var createdOrder = await _orderService.CreateOrder(newOrder);
                return Ok(createdOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetOrders();
            return Ok(orders);
        }

        // Get a specific order by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }
            return Ok(order);
        }


        // Update an order (only by the customer who created it)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(string id, [FromBody] Order updatedOrder)
        {
            try
            {
                var existingOrder = await _orderService.GetOrderById(id);

                if (existingOrder == null)
                {
                    return NotFound(new { message = "Order not found" });
                }

                if (existingOrder.CustomerId != updatedOrder.CustomerId)
                {
                    return Unauthorized(new { message = "You are not authorized to update this order." });
                }

                // Check if the order has already been delivered
                if (existingOrder.Status == 2)
                {
                    return BadRequest(new { message = "Cannot update order. The order has already been delivered." });
                }

                // Update order items
                foreach (var item in updatedOrder.OrderItems)
                {
                    var product = await _productService.GetProductById(item.ProductId);

                    if (product == null)
                    {
                        return BadRequest(new { message = $"Product with ID {item.ProductId} does not exist." });
                    }

                    item.Total = item.UnitPrice * item.Quantity;
                }

                updatedOrder.TotalPrice = updatedOrder.OrderItems.Sum(i => i.Total);
                updatedOrder.OrderItemCount = updatedOrder.OrderItems.Count;
                updatedOrder.Status = existingOrder.Status; // Maintain the current status

                var updatedOrderResult = await _orderService.UpdateOrder(id, updatedOrder);

                if (updatedOrderResult == null)
                {
                    return BadRequest(new { message = "Order update failed." });
                }

                return Ok(updatedOrderResult);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("status/{id}")]
        public async Task<IActionResult> ChangeOrderStatus(string id, [FromBody] int newStatus)
        {
            try
            {
                // Fetch the existing order
                var existingOrder = await _orderService.GetOrderById(id);

                if (existingOrder == null)
                {
                    return NotFound(new { message = "Order not found" });
                }

                // Validate the new status value (0 = Pending, 1 = Processing, 2 = Delivered)
                if (newStatus < 0 || newStatus > 2)
                {
                    return BadRequest(new { message = "Invalid status. Status must be 0 (Pending), 1 (Processing), or 2 (Delivered)." });
                }

                // Prevent changing status back from a higher status (e.g., from Delivered to Processing)
                if (newStatus < existingOrder.Status)
                {
                    return BadRequest(new { message = "Cannot change status back to a previous state." });
                }

                // Update the order status
                existingOrder.Status = newStatus;

                // Update the order in the database
                var updatedOrder = await _orderService.UpdateOrder(id, existingOrder);

                if (updatedOrder == null)
                {
                    return BadRequest(new { message = "Failed to update order status." });
                }

                return Ok(new { message = "Order status updated successfully.", order = updatedOrder });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Customer requests order cancellation
        [HttpPatch("{id}/cancel-request")]
        public async Task<IActionResult> RequestOrderCancellation(string id, [FromBody] string cancellationNote)
        {
            try
            {
                var order = await _orderService.GetOrderById(id);
                if (order == null)
                {
                    return NotFound(new { message = "Order not found" });
                }

                // Only allow cancellation if the order is not yet delivered
                if (order.Status == 2)
                {
                    return BadRequest(new { message = "Cannot cancel a delivered order." });
                }

                // Extract CustomerId from the JWT token
                //var customerIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Ensure that the request is made by the customer who created the order
                //if (customerIdFromToken == null || order.CustomerId != customerIdFromToken)
                //{
                //    return BadRequest(new { message = "You are not authorized to cancel this order." });
                //}

                if (order.IsCancellationRequested)
                {
                    return BadRequest(new { message = "Cancellation already requested." });
                }

                // Request cancellation
                var updatedOrder = await _orderService.RequestOrderCancellation(id, cancellationNote);
                return Ok(new { message = "Cancellation requested successfully.", order = updatedOrder });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Admin approves the cancellation request
        [HttpPatch("{id}/approve-cancellation")]
        public async Task<IActionResult> ApproveOrderCancellation(string id)
        {
            try
            {
                var order = await _orderService.GetOrderById(id);
                if (order == null)
                {
                    return NotFound(new { message = "Order not found" });
                }

                // Ensure a cancellation was requested
                if (!order.IsCancellationRequested)
                {
                    return BadRequest(new { message = "No cancellation request found." });
                }

                // Approve the cancellation
                var updatedOrder = await _orderService.ApproveOrderCancellation(id);
                return Ok(new { message = "Order cancellation approved successfully.", order = updatedOrder });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Get all orders where cancellation has been requested but not yet approved
        [HttpGet("cancel-requests")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCancelRequests()
        {
            try
            {
                var cancelRequests = await _orderService.GetCancelRequests();
                return Ok(cancelRequests);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Get all orders where cancellation has been approved
        [HttpGet("approved-cancellations")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetApprovedCancellations()
        {
            try
            {
                var approvedCancellations = await _orderService.GetApprovedCancellations();
                return Ok(approvedCancellations);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
