using DataTransferObject;

using models;

namespace Service;

public class ProductCategoryService
{
    private readonly ProductCategoryRepository _productCategoryRepository;

    public ProductCategoryService(ProductCategoryRepository productCategoryRepository)
    {
        _productCategoryRepository = productCategoryRepository;
    }

    public async Task<List<Products>> GetProductsByCategoryIdService(int id)
    {
        Task<List<Products>> products = _productCategoryRepository.GetAllProductCategorys(id, "CTE");
        return await products;
    }

    public async Task<List<Products>> GetAllPrevService(int categoryId, int lastId)
    {
        Task<List<Products>> products = _productCategoryRepository.GetProductsPrev(categoryId, lastId, "CTE");
        return await products;
    }

    public async Task<List<Products>> GetAllNextService(int categoryId, int lastId)
    {
        Task<List<Products>> products = _productCategoryRepository.GetProductsNext(categoryId, lastId, "CTE");
        return await products;
    }
}
