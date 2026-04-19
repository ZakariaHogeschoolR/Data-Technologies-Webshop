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

    [HttpGet("next")]
    public async Task<ActionResult<List<Products>>> GetNextProducts([FromQuery] int lastId)
    {
        var products = await _productService.GetAllNextService(lastId);
        return Ok(products);
    }

    [HttpGet("prev")]
    public async Task<ActionResult<List<Products>>> GetPrevProducts([FromQuery] int firstId)
    {
        var products = await _productService.GetAllPrevService(firstId);
        return Ok(products);
    }

    [HttpGet("team/{id}")]
    public async Task<ActionResult<List<Products>>> GetAllProductsByTeam(int id)
    {
        var products = await _productService.GetAllByTeamService(id);
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

    [HttpGet("by-price/{price:double}")]
    public async Task<ActionResult<Products>> GetProductByPrice(double price)
    {
        var product = await _productService.GetByPriceService(price);

        if (product == null)
        {
            return NotFound($"Product with id {price} was not found.");
        }

        return Ok(product);
    }

    [HttpGet("by-name/{name}")]
    public async Task<ActionResult<Products>> GetProductByName(string name)
    {
        var product = await _productService.GetByNameService(name);

        if (product == null)
        {
            return NotFound($"Product with id {name} was not found.");
        }

        return Ok(product);
    }
    
    [HttpGet("search")]
    public async Task<ActionResult<List<Products>>> SearchProducts([FromQuery] string name)
    {
        var products = await _productService.SearchProductsByName(name);
        return Ok(products);
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
