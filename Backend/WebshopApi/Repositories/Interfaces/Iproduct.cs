using DataTransferObject;

using models;

public interface IProduct
{
    Task<List<Products>> GetAllProducts();

    Task<List<Products>> GetAllProductsForGraph();

    Task<List<Products>> GetAllProductsAdmin();

    Task<List<Products>> GetAllProductsAdminPaged(int page, int pageSize);

    Task<List<Products>> GetProductsPrev(int lastId);

    Task<List<Products>> GetProductsNext(int lastId);

    Task<List<Products>> GetAllProductsByTeam(int id);

    Task<List<Products>> SearchProductsByName(string name);

    Task<Products?> GetProductById(int id);

    Task<List<Products?>> GetProductByPrice(double price);

    Task<List<Products?>> GetProductByName(string name);

    Task<List<Products?>> GetProductsByPrice(double price);

    Task<List<Products>> GetProductsByCategories(List<int> categoryIds, int page, int pageSize);

    Task AddProduct(ProductDto product);

    Task<int> AddProductScrape(ProductDto product);

    Task UpdateProduct(ProductDto product);

    Task UpdatePrice(int id, decimal price);

    Task<int> GetOrCreateTeam(string name, string type);

    Task<int> GetOrCreateCategory(string name);

    Task AddProductCategory(int productId, int categoryId);

    Task DeleteProduct(int id);
}
