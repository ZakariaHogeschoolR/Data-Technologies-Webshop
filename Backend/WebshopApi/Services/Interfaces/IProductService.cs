using DataTransferObject;

using models;

public interface IProductService
{
    Task<List<Products>> GetAllService();

    Task<List<Products>> GetAllServiceAdmin();

    Task<List<Products>> GetAllPrevService(int lastId);

    Task<List<Products>> GetAllNextService(int lastId);

    Task<List<Products>> GetAllByTeamService(int id);

    Task<List<Products>> GetAllServiceAdminPaged(int page, int pageSize);

    Task<List<Products>> SearchProductsByName(string name);

    Task<Products> GetByIdService(int id);

    Task<List<Products>> GetByPriceService(double price);

    Task<List<Products>> GetByNameService(string name);

    Task<List<Products>> GetProductsByCategoriesService(List<int> categoryIds, int page, int pageSize);

    Task AddProductCategoryService(int productId, int categoryId);

    Task CreateService(ProductDto product);

    Task<int> CreateServiceReturnId(ProductDto product);

    Task UpdateService(ProductDto product);

    Task UpdatePriceService(int id, decimal price);

    Task DeleteService(int id);
}
