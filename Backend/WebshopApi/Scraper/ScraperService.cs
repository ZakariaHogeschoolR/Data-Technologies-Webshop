using DataTransferObject;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Service
{
    public class ScraperService
    {
        private readonly ProductRepository _productRepository;
        private readonly HttpClient _http;

        public ScraperService(ProductRepository productRepository)
        {
            _productRepository = productRepository;
            _http = new HttpClient();
            _http.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
        }
        
        public async Task ImportFromApiAsync()
        {
            var response = await _http.GetStringAsync("https://fakestoreapi.com/products");
            var items = JsonSerializer.Deserialize<List<FakeStoreProduct>>(response);

            foreach (var item in items)
            {
                var product = new ProductDto
                {
                    Name         = item.Title,
                    Price        = (decimal)item.Price,
                    Description  = item.Description?.Length > 250 
                                    ? item.Description[..250] 
                                    : item.Description,
                    ProductImage = item.ProductImage
                };
                await _productRepository.AddProduct(product);
            }
        }

        // DTO voor de API response
        public class FakeStoreProduct
        {
            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("price")]
            public double Price { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("image")]
            public string ProductImage { get; set; }
        }
    }
}