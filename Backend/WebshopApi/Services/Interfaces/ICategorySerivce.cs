using ApplicationDbContext;

using DataTransferObject;

using models;

using Npgsql;

public interface ICategoryService
{
    Task<List<Categories>> GetAllService();

    Task<Categories> GetByIdService(int id);

    // Task<List<Categories>> GetByPriceService(double price);

    // Task<List<Categories>> GetByNameService(string name);

    Task CreateService(CategoryDto category);

    Task UpdateService(CategoryDto category);

    Task DeleteService(int id);
}
