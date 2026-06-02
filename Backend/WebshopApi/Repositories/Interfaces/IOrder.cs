using ApplicationDbContext;

using DataTransferObject;

using models;

using Npgsql;

public interface IOrder
{
    Task<List<Orders>> GetAllOrders();

    Task<Orders?> GetOrderByWinkelwagenUsersId(int id);

    Task<Orders?> GetOrderById(int id);


    Task AddOrder(OrderDto order);

    Task UpdateOrder(OrderDto order);

    Task DeleteOrderWinkelwagen(int id);

    Task DeleteOrder(int id);
}