using AudiophileAPI.DataAccess.EF.Models;
using AudiophileAPI.DataAccess.EF.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AudiophileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {

        private readonly OrderDetailRepository _orderDetailRepository;

        public OrderDetailController(OrderDetailRepository orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        [HttpGet("find-by-id/{id}")]
        public async Task<ActionResult<OrderDetail>> GetOrderDetail(int id)
        {
            try
            {
                var orderDetail = await _orderDetailRepository.GetOrderDetailById(id);

                if (orderDetail == null)
                {
                    return NotFound(new
                    {
                        Message = $"Details with ID {id} was not found."
                    });
                }

                return Ok(orderDetail);
            }
            catch (Exception ex)
            {
                return Problem(
                     detail: ex.Message,
                     title: "An error occurred while fetching order detail.",
                     statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
