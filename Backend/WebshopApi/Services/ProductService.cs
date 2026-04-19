using DataTransferObject;

using models;

namespace Service;

public class ProductService
{
    private readonly ProductRepository _productRepository;

    public ProductService(ProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<Products>> GetAllService()
    {
        Task<List<Products?>> products = _productRepository.GetAllProducts();
        return await products;
    }

    public async Task<List<Products>> GetAllServiceAdmin()
    {
        Task<List<Products?>> products = _productRepository.GetAllProductsAdmin();
        return await products;
    }

    public async Task<List<Products>> GetAllPrevService(int lastId)
    {
        Task<List<Products?>> products = _productRepository.GetProductsPrev(lastId);
        return await products;
    }

    public async Task<List<Products>> GetAllNextService(int lastId)
    {
        Task<List<Products?>> products = _productRepository.GetProductsNext(lastId);
        return await products;
    }

    public async Task<List<Products>> GetAllByTeamService(int id)
    {
        Task<List<Products?>> products = _productRepository.GetAllProductsByTeam(id);
        return await products;
    }

    public async Task<List<Products>>SearchProductsByName(string name)
    {
        Task<List<Products>> products = _productRepository.SearchProductsByName(name);
        return await products;
    }

    public async Task<Products> GetByIdService(int id)
    {
        Task<Products?> product = _productRepository.GetProductById(id);
        return await product;
    }

    public async Task<List<Products>> GetByPriceService(double price)
    {
        Task<List<Products?>> product = _productRepository.GetProductByPrice(price);
        return await product;
    }

    public async Task<List<Products>> GetByNameService(string name)
    {
        Task<List<Products?>> product = _productRepository.GetProductByName(name);
        return await product;
    }

    public async Task CreateService(ProductDto product)
    {
        await _productRepository.AddProduct(product);
    }

    public async Task UpdateService(ProductDto product)
    {
        await _productRepository.UpdateProduct(product);
    }

    public async Task DeleteService(int id)
    {
        await _productRepository.DeleteProduct(id);
    }
}
