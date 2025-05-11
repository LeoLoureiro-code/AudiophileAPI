using AudiophileAPI.DataAccess.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiophileAPI.DataAccess.EF.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrders();

        Task<Order> GetOrderById(int id);

        Task<Order> ChangeOrderStatus(int id, string orderStatus);

        Task<Order> AddOrder(Order order);

        Task DeleteOrder(int id);

    }
}
