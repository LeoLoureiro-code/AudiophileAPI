using AudiophileAPI.DataAccess.EF.Context;
using AudiophileAPI.DataAccess.EF.Interfaces;
using AudiophileAPI.DataAccess.EF.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiophileAPI.DataAccess.EF.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AudiophileAPIDbContext _context;

        public OrderRepository(AudiophileAPIDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<Order> GetOrderById(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                throw new Exception("Order not found");
            }

            return order;
        }

        public async Task<Order> AddOrder(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> ChangeOrderStatus(int id, string orderStatus)
        {
            Order existingOrder = await _context.Orders.FindAsync(id);

            if (existingOrder == null)
            {
                throw new Exception("Order not found");
            }

            existingOrder.Status = orderStatus;
            await _context.SaveChangesAsync();
            return existingOrder;

        }

        public async Task DeleteOrder(int id)
        {
            Order order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                throw new Exception("Product not found");
            }
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}
