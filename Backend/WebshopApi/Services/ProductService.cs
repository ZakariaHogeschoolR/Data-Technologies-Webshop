using System.Threading.Tasks;
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
            Task<List<Products?>> products = _productRepository.GetAllProducts();
            return await products;
        }

        public async Task<Products> GetByIdService(int id)
        {
            Task<Products?> product = _productRepository.GetProductById(id);
            return await product;
        }

        public async Task<List<Products>> GetByPriceService(double price)
        {
            Task<List<Products?>> products = _productRepository.GetProductsByPrice(price);
            return await products;
        }

        public void CreateService(ProductDto product)
        {
            _productRepository.AddProduct(product);
        }

        public void UpdateService(ProductDto product)
        {
            _productRepository.UpdateProduct(product);
        }

        public void DeleteService(int id)
        {
            _productRepository.DeleteProduct(id);
        }
    }
    
}