/************************************************************
 * File:        IOrderService.cs
 * Author:      IT21252754 Madhumalka K.C.S
 * Date:        2024-09-23
 * Description: This interface defines the contract for managing 
 *              orders, including creating, retrieving, updating, 
 *              and handling cancellation requests. It also includes
 *              methods to retrieve orders by vendor.
 ************************************************************/

using WebService.Models;

namespace WebService.Interfaces
{
    public interface IOrderService
    {
        /// Creates a new order and returns the created order object.
        Task<Order> CreateOrder(Order newOrder);

        /// Retrieves all orders.
        Task<List<Order>> GetOrders();

        // Retrieves a specific order by its ID.
        Task<Order?> GetOrderById(string id);

        /// Updates an existing order and returns the updated order.
        Task<Order?> UpdateOrder(string id, Order updatedOrder);

        /// Marks an order as having a cancellation request and returns the updated order.
        Task<Order?> RequestOrderCancellation(string id, string cancellationNote);

        /// Approves the cancellation request for an order and returns the updated order.
        Task<Order?> ApproveOrderCancellation(string id);

        /// Rejects the cancellation request for an order and returns the updated order.
        Task<Order?> RejectOrderCancellation(string id);

        /// Retrieves all orders that have a pending cancellation request.
        Task<List<Order>> GetCancelRequests();

        /// Retrieves all orders where the cancellation request has been approved.
        Task<List<Order>> GetApprovedCancellations();

        /// Retrieves all orders associated with a specific vendor by vendor ID.
        Task<List<Order>> GetOrdersByVendorId(string vendorId);

        /// Retrieves all orders associated with a specific customer by customer ID.
        Task<List<Order>> GetOrdersByCustromerId(string customerId);

        // Retrieves the last order based on the OrderCode or ID.
        Task<Order?> GetLastOrder();

    }
}
