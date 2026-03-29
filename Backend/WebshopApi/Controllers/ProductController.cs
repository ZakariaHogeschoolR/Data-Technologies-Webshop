using Microsoft.AspNetCore.Mvc;
using models;
using Service;
using DataTransferObject;

[ApiController]
[Route("api/[controller]")]
public class ProductController: ControllerBase
{
    private readonly ProductService _productService;
    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet()]
    public async Task<ActionResult<List<Products>>> GetAllProducts()
    {
        var products = await _productService.GetAllService();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Products>> GetAllProductsById(int id)
    {
        var product = await _productService.GetByIdService(id);
        return Ok(product);
    }
    
    [HttpGet("by-price/{price: double}")]
    public async Task<ActionResult<List<Products>>> GetAllProductsByPrice(double price)
    {
        var products = await _productService.GetByPriceService(price);
        return Ok(products);
    }

    [HttpPost("create")]
    public async Task<ActionResult> CreateProduct(ProductDto product)
    {
        _productService.CreateService(product);
        return Ok();
    }

    [HttpPost("Update")]
    public async Task<ActionResult> UpdateProduct(ProductDto product)
    {
        _productService.UpdateService(product);
        return Ok();
    }

    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        _productService.DeleteService(id);
        return Ok();
    }
}