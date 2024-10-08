﻿/************************************************************
 * File:        OrdersController.cs
 * Author:      IT21252754 Madhumalka K.C.S
 * Date:        2024-09-23
 * Description: This controller handles API operations for orders,
 *              including creation, updating, and management of
 *              order status and cancellation requests. The controller
 *              also allows vendors to mark items as delivered.
 ************************************************************/

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
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;

        public OrdersController(IOrderService orderService, IProductService productService, INotificationService notificationService, IUserService userService)
        {
            _orderService = orderService;
            _productService = productService;
            _notificationService = notificationService;
            _userService = userService;
        }

        /// Creates a new order. Sets the total price and initializes status.
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order newOrder)
        {
            try
            {
                // Generate OrderCode
                var lastOrder = await _orderService.GetLastOrder();
                if (lastOrder != null && !string.IsNullOrEmpty(lastOrder.OrderCode))
                {
                    // Extract numeric part of the last OrderCode
                    var lastOrderCodeNumeric = int.Parse(lastOrder.OrderCode.Replace("ORD#", ""));
                    // Increment the numeric part to generate a new OrderCode
                    newOrder.OrderCode = $"ORD#{lastOrderCodeNumeric + 1}";
                }
                else
                {
                    // If no previous order, start with ORD#1000
                    newOrder.OrderCode = "ORD#1000";
                }

                // Set the total price for each item and the order
                foreach (var item in newOrder.OrderItems)
                {
                    item.Total = item.UnitPrice * item.Quantity;

                    // Reduce product quantity from the stock
                    await _productService.ReduceStockById(item.ProductId, item.Quantity);
                }

                newOrder.TotalPrice = newOrder.OrderItems.Sum(i => i.Total);
                newOrder.Date = DateTime.UtcNow;
                newOrder.OrderItemCount = newOrder.OrderItems.Count;

                // Initially, the status is "Pending"
                newOrder.Status = 0;

                // Save the new order in the database
                var createdOrder = await _orderService.CreateOrder(newOrder);
                return Ok(createdOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// Retrieves all orders.
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

            return Ok(new
            {
                order.Id,
                order.CustomerId,
                CustomerFirstName = order.CustomerFirstName,
                CustomerLastName = order.CustomerLastName,
                order.Date,
                order.OrderCode,
                order.TotalPrice,
                order.Status,
                OrderItems = order.OrderItems.Select(i => new
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    VendorId = i.VendorId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Total = i.Total,
                    IsDelivered = i.IsDelivered
                }),
                order.OrderItemCount,
                order.IsCancellationRequested,
                order.CancellationNote,
                order.IsCancellationApproved,
                order.Recipient_Name,
                order.Recipient_Email,
                order.Recipient_Contact,
                order.Recipient_Address
            });

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

                foreach (var item in updatedOrder.OrderItems)
                {
                    item.Total = item.UnitPrice * item.Quantity;
                }

                updatedOrder.TotalPrice = updatedOrder.OrderItems.Sum(i => i.Total);
                updatedOrder.OrderItemCount = updatedOrder.OrderItems.Count;

                // Update the order status based on delivery of products
                UpdateOrderStatusBasedOnItems(updatedOrder);

                // Save the updated order in the database
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

        /// Changes the status of an order (0 = Pending, 1 = Processing, 2 = Delivered).
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

                var customer = await _userService.GetUserById(order.CustomerId);

                var notification = new Notification
                {
                    IsVisibleToCSR = true,
                    Message = $"Customer :'{customer?.First_Name} {customer?.Last_Name}' has requested to cancel Order ID: {order.Id} placed on {order.Date}. Please review the request and take appropriate action."
                };
                await _notificationService.CreateNotification(notification);

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

        /// Rejects the order cancellation request.
        [HttpPatch("{id}/reject-cancellation")]
        public async Task<IActionResult> RejectOrderCancellation(string id)
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
                var updatedOrder = await _orderService.RejectOrderCancellation(id);
                return Ok(new { message = "Order cancellation reject successfully.", order = updatedOrder });
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

        // Vendor marks a product as delivered
        [HttpPatch("{orderId}/vendor/{vendorId}/product/{productId}/deliver")]
        public async Task<IActionResult> MarkProductAsDelivered(string orderId, string vendorId, string productId)
        {
            try
            {
                var order = await _orderService.GetOrderById(orderId);
                if (order == null)
                {
                    return NotFound(new { message = "Order not found" });
                }

                // Find the order item belonging to the given vendor and product
                var orderItem = order.OrderItems.FirstOrDefault(item => item.ProductId == productId && item.VendorId == vendorId);
                if (orderItem == null)
                {
                    return BadRequest(new { message = "Product not found or vendor is unauthorized to update this product." });
                }

                // If the item is already delivered, return an error
                if (orderItem.IsDelivered)
                {
                    return BadRequest(new { message = "Product has already been marked as delivered." });
                }

                // Mark the product as delivered
                orderItem.IsDelivered = true;

                // Check if all items have been delivered
                bool allItemsDelivered = order.OrderItems.All(item => item.IsDelivered);

                // Check if some items are delivered
                bool someItemsDelivered = order.OrderItems.Any(item => item.IsDelivered);

                // Update the order status based on delivery status of all items
                if (allItemsDelivered)
                {
                    order.Status = 2; // Set status to 'Delivered'

                    var notification = new Notification
                    {
                        ReceiverId = order.CustomerId,
                        Message = $"Your order '{order.OrderCode}' has been delivered! Thank you for shopping with us."
                    };
                    await _notificationService.CreateNotification(notification);
                }
                else if (someItemsDelivered)
                {
                    order.Status = 1; // Set status to 'Partially Delivered'
                }

                var updatedOrder = await _orderService.UpdateOrder(orderId, order);

                if (updatedOrder == null)
                {
                    return BadRequest(new { message = "Failed to update order status." });
                }

                return Ok(new { message = "Product marked as delivered.", order = updatedOrder });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Updates the status of each order item based on delivery status.
        private void UpdateOrderStatusBasedOnItems(Order order)
        {
            // Check if all products in the order have been delivered
            bool allItemsDelivered = order.OrderItems.All(item => item.IsDelivered);

            // If all items are delivered, mark the order as "Delivered"
            if (allItemsDelivered)
            {
                order.Status = 2; // Delivered
            }
            else if (order.OrderItems.Any(item => item.IsDelivered))
            {
                // If some items are delivered, mark the order as "Partially Delivered"
                order.Status = 1; // Partially Delivered
            }
            else
            {
                // If no items are delivered, keep the order as "Pending"
                order.Status = 0; // Pending
            }
        }

        //retrie orders based on vendor
        [HttpGet("vendor/{vendorId}")]
        public async Task<IActionResult> GetOrdersByVendorId(string vendorId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByVendorId(vendorId);

                if (orders == null || !orders.Any())
                {
                    return NotFound(new { message = "No orders found for this vendor." });
                }

                return Ok(orders.Select(order => new
                {
                    order.Id,
                    order.CustomerId,
                    order.CustomerFirstName,
                    order.CustomerLastName,
                    order.Date,
                    order.OrderCode,
                    order.TotalPrice,
                    order.Status,
                    order.OrderItems,
                    order.OrderItemCount,
                    order.IsCancellationRequested,
                    order.CancellationNote,
                    order.IsCancellationApproved
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //retrie orders based on customer
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetOrdersByCustomerId(string customerId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByCustromerId(customerId);

                if (orders == null || !orders.Any())
                {
                    return NotFound(new { message = "No orders found for this customer." });
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
