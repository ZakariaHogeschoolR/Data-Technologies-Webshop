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
        var products = _productService.GetAllService();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Products>> GetAllProductsById(int id)
    {
        var product = _productService.GetByIdService(id);
        return Ok(product);
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