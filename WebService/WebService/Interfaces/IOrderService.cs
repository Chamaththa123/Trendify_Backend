using WebService.Models;

namespace WebService.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrder(Order newOrder);
        Task<List<Order>> GetOrders();
        Task<Order?> GetOrderById(string id);
        Task<Order?> UpdateOrder(string id, Order updatedOrder);
        Task<Order?> RequestOrderCancellation(string id, string cancellationNote);
        Task<Order?> ApproveOrderCancellation(string id);
        Task<Order?> RejectOrderCancellation(string id);
        Task<List<Order>> GetCancelRequests();
        Task<List<Order>> GetApprovedCancellations();

        Task<List<Order>> GetOrdersByVendorId(string vendorId);
    }
}
