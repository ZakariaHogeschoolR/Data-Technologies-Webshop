using Microsoft.AspNetCore.Mvc;
using models;
using Service;
using DataTransferObject;

[ApiController]
[Route("api/[controller]")]
public class ProductCategoryController : ControllerBase
{
    private readonly ProductCategoryService _productCategoryService;

    public ProductCategoryController(ProductCategoryService productcategoryService)
    {
        _productCategoryService = productcategoryService;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Products>> GetProductsByCategoryId(int id)
    {
        var productCategory = await _productCategoryService.GetProductsByCategoryIdService(id);

        if (productCategory == null)
        {
            return NotFound($"Product with id {id} was not found.");
        }

        return Ok(productCategory);
    }

    [HttpGet("next")]
    public async Task<ActionResult<List<Products>>> GetNextProducts([FromQuery] int categoryId, [FromQuery] int lastId)
    {
        var products = await _productCategoryService.GetAllNextService(categoryId, lastId);
        return Ok(products);
    }

    [HttpGet("prev")]
    public async Task<ActionResult<List<Products>>> GetPrevProducts([FromQuery] int categoryId, [FromQuery] int firstId)
    {
        var products = await _productCategoryService.GetAllPrevService(categoryId, firstId);
        return Ok(products);
    }
}