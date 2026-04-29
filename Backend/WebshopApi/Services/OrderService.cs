using DataTransferObject;

using models;

namespace Service;

public class OrderService
{
    private readonly OrderRepository _orderRepository;

    public OrderService(OrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<List<Orders>> GetAllService()
    {
        Task<List<Orders?>> orders = _orderRepository.GetAllOrders();
        return await orders;
    }

    public async Task<Orders> GetByIdService(int id)
    {
        Task<Orders?> order = _orderRepository.GetOrderById(id);
        return await order;
    }

    public async Task<Orders> GetByWinkelwagenUsersIdService(int id)
    {
        Task<Orders?> order = _orderRepository.GetOrderByWinkelwagenUsersId(id);
        return await order;
    }

    public async Task CreateService(OrderDto order)
    {
        await _orderRepository.AddOrder(order);
    }

    public async Task UpdateService(OrderDto order)
    {
        await _orderRepository.UpdateOrder(order);
    }
    
    public async Task DeleteOrderService(int id)
    {
        await _orderRepository.DeleteOrderWinkelwagen(id);
    }

    public async Task DeleteService(int id)
    {
        await _orderRepository.DeleteOrder(id);
    }
}
