using MB_Project.Data;
using MB_Project.IRepos;
using MB_Project.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using static NuGet.Packaging.PackagingConstants;

namespace MB_Project.Repos
{
    public class OrderRepo : IOrderRepo
    {
        private readonly MB_ProjectContext _context;

        public OrderRepo(MB_ProjectContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            try
            {
                var orderList = await _context.Orders
                    .Select
                    (
                        r => new Order
                        {
                            Id = r.Id,
                            PostId = r.PostId,
                            UserId = r.UserId,
                            TotalPrice = r.TotalPrice,
                            OrderStatus = r.OrderStatus,
                            /*
                            PostFeatures = r.PostFeatures.Select ( f => new PostFeature
                            {
                                Id = f.Id,
                                PostId = f.PostId,
                                Title = f.Title,
                                Price = f.Price
                            }).ToList ()
                            */
                        }
                    )
                    .ToListAsync(); // .include postFeatures
                return orderList;
            }
            catch
            {
                return Enumerable.Empty<Order>();
            }
        }
        public async Task<Order> GetOrderById(int OrderId)
        {
            try
            {
                var order = await _context.Orders
                    .Where(x => x.Id == OrderId)
                    .Select
                    (
                        r => new Order
                        {
                            Id = r.Id,
                            PostId = r.PostId,
                            UserId = r.UserId,
                            TotalPrice = r.TotalPrice,
                            OrderStatus = r.OrderStatus,

                            PostFeatures = r.PostFeatures.Select(f => new PostFeature
                            {
                                Id = f.Id,
                                PostId = f.PostId,
                                Title = f.Title,
                                Price = f.Price
                            }).ToList()
                        }
                    )
                    .FirstOrDefaultAsync();
                return order;
            }
            catch
            {
                return null;
            }
        }
        public async Task<IEnumerable<Order>> GetPostOrders(int PostId)
        {
            try
            {
                var orderList = await _context.Orders
                    .Where(x => x.PostId == PostId)
                    .Select
                    (
                        r => new Order
                        {
                            Id = r.Id,
                            PostId = r.PostId,
                            UserId = r.UserId,
                            TotalPrice = r.TotalPrice,
                            OrderStatus = r.OrderStatus,
                            /*
                            PostFeatures = r.PostFeatures.Select(f => new PostFeature
                            {
                                Id = f.Id,
                                PostId = f.PostId,
                                Title = f.Title,
                                Price = f.Price
                            }).ToList()
                            */
                        }
                    )
                    .ToListAsync();
                return orderList;
            }
            catch
            {
                return Enumerable.Empty<Order>();
            }
        }
        public async Task<IEnumerable<Order>> GetUserOrders(string UserId)
        {
            try
            {
                var orderList = await _context.Orders
                    .Where(x => x.UserId == UserId)
                    .Select
                    (
                        r => new Order
                        {
                            Id = r.Id,
                            PostId = r.PostId,
                            UserId = r.UserId,
                            TotalPrice = r.TotalPrice,
                            OrderStatus = r.OrderStatus,
                            /*
                            PostFeatures = r.PostFeatures.Select(f => new PostFeature
                            {
                                Id = f.Id,
                                PostId = f.PostId,
                                Title = f.Title,
                                Price = f.Price
                            }).ToList()
                            */
                        }
                    )
                    .ToListAsync();
                return orderList;
            }
            catch
            {
                return Enumerable.Empty<Order>();
            }
        }
        public async Task<IEnumerable<Order>> GetSellerProjects(string sellerId)
        {
            try
            {
                /*
                var posts = await _context.Posts.Where(x=>x.FreelancerId == sellerId).ToListAsync();
                if (posts.Count() == 0 )
                {
                    return Enumerable.Empty<Order>();
                }
                */
                var orders = await _context.Orders
                            .Where(order => _context.Posts
                            .Any(post => post.Id == order.PostId && post.FreelancerId == sellerId))
                            .ToListAsync();
                return orders;
            }
            catch
            {
                return Enumerable.Empty<Order>();
            }
        }
        public async Task<bool> CreateOrder(Order order)
        {
            try
            {
                var chk = await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch 
            { 
                return false; 
            }
        }
        public async Task<bool> UpdateOrder(int OrderId, Order order)
        {
            try
            {
                var obj = await _context.Orders.FindAsync(OrderId);
                if(obj == null)
                {
                    return false;
                }
                obj.PostId = order.PostId;
                var pstFet = await _context.PostFeatures.Where(x => x.PostId == obj.PostId).ToListAsync();
                obj.PostFeatures = pstFet;
                obj.TotalPrice = 1000; // edit this for example
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> DeleteOrder(int OrderId)
        {
            try
            {
                var order = await _context.Orders.FindAsync(OrderId);
                if (order == null)
                {
                    return false;
                }
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> UpdateOrderStatus(int OrderId, string Status)
        {
            try
            {
                var order = await _context.Orders.FindAsync(OrderId);
                if(order == null)
                {
                    return false;
                }
                
                    order.OrderStatus = Status;
                    await _context.SaveChangesAsync();
                    return true;
                
            }
            catch
            {
                return false;
            }
        }
    }
}
