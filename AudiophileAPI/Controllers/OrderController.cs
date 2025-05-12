using AudiophileAPI.DataAccess.EF.Interfaces;
using AudiophileAPI.DataAccess.EF.Models;
using AudiophileAPI.DTO;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AudiophileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet("all-orders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            try
            {
                var order = await _orderRepository.GetAllOrders();
                if (order == null || !order.Any())
                {
                    return NotFound("No orders found");
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    title: "An error occurred while fetching orders.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("find-by-id/{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            try
            {
                var order = await _orderRepository.GetOrderById(id);

                if (order == null)
                {
                    return NotFound(new
                    {
                        Message = $"Order with ID {id} was not found."
                    });
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                return Problem(
                     detail: ex.Message,
                     title: "An error occurred while fetching order.",
                     statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("create-order")]
        public async Task<ActionResult> Createorder([FromBody] Order order)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Message = "All fields are required."
                    });
                }
                var createdOrder = await _orderRepository.AddOrder(order);

                return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.OrderId }, createdOrder);
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    title: "An error occurred while creating the order.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("update-user/{id}")]
        public async Task<ActionResult> UpdateOrderStatus(int id, [FromBody] Order order)
        {
            try
            {
                if (order.OrderId != 0 && order.OrderId != id)
                {
                    return BadRequest("Order ID in the body does not match URL.");
                }

                var updated = await _orderRepository.ChangeOrderStatus(id, order.Status);

                if (updated == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    title: "An error occurred while updating the order.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("delete-order/{id}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            try
            {
                var order = await _orderRepository.GetOrderById(id);
                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }

                await _orderRepository.DeleteOrder(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    title: "An error occurred while deleting the order.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
