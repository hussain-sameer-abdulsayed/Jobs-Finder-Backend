using MB_Project.Models;

namespace MB_Project.IRepos
{
    public interface IOrderRepo
    {
        Task<IEnumerable<Order>> GetAllOrders();
        Task<IEnumerable<Order>> GetUserOrders(string UserId);
        Task<IEnumerable<Order>> GetSellerProjects(string sellerId);
        Task<IEnumerable<Order>> GetPostOrders(int PostId);
        Task<Order> GetOrderById(int OrderId);
        Task<bool> DeleteOrder(int OrderId);
        Task<bool> CreateOrder(Order order);
        Task<bool> UpdateOrder(int OrderId,Order order);
        Task<bool> UpdateOrderStatus(int OrderId, string Status);
    }
}
