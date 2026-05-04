using DataTransferObject;

using Microsoft.AspNetCore.Mvc;

using models;

using Service;

namespace WebshopApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrderController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Orders>>> GetAllOrders()
    {
        var orders = await _orderService.GetAllService();
        return Ok(orders);
    }

    [HttpGet("winkelwagen/{id}")]
    public async Task<ActionResult<Orders>> GetOrderById(int id)
    {
        var order = await _orderService.GetByIdService(id);

        return Ok(order);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Orders>> GetOrderBywinkelwagenUsersId(int id)
    {
        var order = await _orderService.GetByWinkelwagenUsersIdService(id);

        if (order == null) return NotFound($"Order with id {id} was not found.");

        return Ok(order);
    }

    [HttpPost("create")]
    public async Task<ActionResult> CreateOrder([FromBody] OrderDto order)
    {
        //if (order == null) return BadRequest("Order data is required.");

        await _orderService.CreateService(order);
        return Ok("Order created successfully.");
    }

    [HttpPut("update")]
    public async Task<ActionResult> UpdateOrder([FromBody] OrderDto order)
    {
        if (order == null || order.Id <= 0) return BadRequest("A valid order id is required.");

        var existingOrder = await _orderService.GetByIdService(order.Id);
        if (existingOrder == null) return NotFound($"Order with id {order.Id} was not found.");

        await _orderService.UpdateService(order);
        return Ok("Order updated successfully.");
    }

    [HttpDelete("winkelwagen/delete/{id}")]
    public async Task<ActionResult> DeleteOrderWinkelwagen(int id)
    {
        await _orderService.DeleteOrderService(id);
        return Ok("Order deleted successfully.");
    }

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> DeleteOrder(int id)
    {
        var existingOrder = await _orderService.GetByIdService(id);
        if (existingOrder == null) return NotFound($"Order with id {id} was not found.");

        await _orderService.DeleteService(id);
        return Ok("Order deleted successfully.");
    }
}
