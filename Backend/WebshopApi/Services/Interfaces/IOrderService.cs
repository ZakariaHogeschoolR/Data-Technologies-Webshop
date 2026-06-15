using ApplicationDbContext;

using DataTransferObject;

using models;

using Npgsql;

public interface IOrderService
{
    Task<List<Orders>> GetAllService();

    Task<Orders> GetByIdService(int id);

    Task<Orders> GetByWinkelwagenUsersIdService(int id);

    Task CreateService(OrderDto order);

    Task UpdateService(OrderDto order);

    Task DeleteOrderService(int id);

    Task DeleteService(int id);
}
