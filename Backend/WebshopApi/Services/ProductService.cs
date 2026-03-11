using System.Threading.Tasks;
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
            Task<List<Products?>> users = _productRepository.GetAllProducts();
            return await users;
        }

        public async Task<Products> GetByIdService(int id)
        {
            Task<Products?> user = _productRepository.GetProductById(id);
            return await user;
        }

        public void CreateService(Products product)
        {
            _productRepository.AddProduct(product);
        }

        public void UpdateService(Products product)
        {
            _productRepository.UpdateProduct(product);
        }

        public void DeleteService(int id)
        {
            _productRepository.DeleteProduct(id);
        }
    }
    
}