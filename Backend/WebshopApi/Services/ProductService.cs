using DataTransferObject;
using models;

namespace Service
{
    public class ProductService
    {
        private readonly ProductRepository _productRepository;

        public ProductService(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<Products>> GetAllService()
        {
            return await _productRepository.GetAllProducts();
        }

        public async Task<Products?> GetByIdService(int id)
        {
            return await _productRepository.GetProductById(id);
        }
      
        public async Task<List<Products>> GetByPriceService(double price)
        {
            Task<List<Products?>> products = _productRepository.GetProductsByPrice(price);
            return await products;
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
}