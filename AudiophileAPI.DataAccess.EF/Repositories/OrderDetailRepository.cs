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
    public class OrderDetailRepository: IOrderDetailRepository
    {
        private readonly AudiophileAPIDbContext _context;

        public OrderDetailRepository(AudiophileAPIDbContext context)
        {
            _context = context;
        }

        public async Task<OrderDetail> GetOrderDetailById(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);

            if (orderDetail == null)
            {
                throw new Exception("Order not found");
            }

            return orderDetail;
        }

    }
}
