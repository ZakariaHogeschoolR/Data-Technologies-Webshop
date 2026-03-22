using DataTransferObject;

using Microsoft.AspNetCore.Mvc;

using models;

using Service;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Products>>> GetAllProducts()
    {
        var products = await _productService.GetAllService();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Products>> GetProductById(int id)
    {
        var product = await _productService.GetByIdService(id);

        if (product == null)
        {
            return NotFound($"Product with id {id} was not found.");
        }

        return Ok(product);
    }

    [HttpPost("create")]
    public async Task<ActionResult> CreateProduct([FromBody] ProductDto product)
    {
        if (product == null)
        {
            return BadRequest("Product data is required.");
        }

        await _productService.CreateService(product);
        return Ok("Product created successfully.");
    }

    [HttpPut("update")]
    public async Task<ActionResult> UpdateProduct([FromBody] ProductDto product)
    {
        if (product == null || product.Id <= 0)
        {
            return BadRequest("A valid product id is required.");
        }

        var existingProduct = await _productService.GetByIdService(product.Id);
        if (existingProduct == null)
        {
            return NotFound($"Product with id {product.Id} was not found.");
        }

        await _productService.UpdateService(product);
        return Ok("Product updated successfully.");
    }

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var existingProduct = await _productService.GetByIdService(id);
        if (existingProduct == null)
        {
            return NotFound($"Product with id {id} was not found.");
        }

        await _productService.DeleteService(id);
        return Ok("Product deleted successfully.");
    }
}
