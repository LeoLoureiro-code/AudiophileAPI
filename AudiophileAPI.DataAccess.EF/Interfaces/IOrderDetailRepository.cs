using AudiophileAPI.DataAccess.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiophileAPI.DataAccess.EF.Interfaces
{
    public interface IOrderDetailRepository
    {

        Task<OrderDetail> GetOrderDetailById(int id);
    }
}
