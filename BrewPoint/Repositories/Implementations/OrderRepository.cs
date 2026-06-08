using BrewPoint.Data;
using BrewPoint.Models;
using BrewPoint.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BrewPoint.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task PlaceOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
                return await _context.Orders
           .Include(o => o.User)
           .Include(o => o.Items)
               .ThenInclude(i => i.Coffee)
           .Include(o => o.Items)
               .ThenInclude(i => i.Extras)
                   .ThenInclude(e => e.Ingredient)  
           .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserId(string userId)
        {
                    return await _context.Orders
             .Include(o => o.User)
             .Include(o => o.Items)
                 .ThenInclude(i => i.Coffee)
             .Include(o => o.Items)
                 .ThenInclude(i => i.Extras)
                     .ThenInclude(e => e.Ingredient)  
             .Where(o => o.UserId == userId)
             .ToListAsync();
        }

        public async Task<Order?> GetOrderById(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Extras)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrder(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }
    }
}