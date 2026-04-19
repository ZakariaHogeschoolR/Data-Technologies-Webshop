using Microsoft.AspNetCore.Mvc;
using models;
using Service;
using DataTransferObject;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Categories>>> GetAllCategories()
    {
        var categories = await _categoryService.GetAllService();
        return Ok(categories);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Categories>> GetProductById(int id)
    {
        var category = await _categoryService.GetByIdService(id);

        if (category == null)
        {
            return NotFound($"Product with id {id} was not found.");
        }

        return Ok(category);
    }

    [HttpGet("by-price/{price:double}")]
    public async Task<ActionResult<Categories>> GetCategoryByPrice(double price)
    {
        var Category = await _categoryService.GetByPriceService(price);

        if (Category == null)
        {
            return NotFound($"Category with id {price} was not found.");
        }

        return Ok(Category);
    }

    [HttpPost("create")]
    public async Task<ActionResult> CreateCategory([FromBody] CategoryDto category)
    {
        if (category == null)
        {
            return BadRequest("Category data is required.");
        }

        await _categoryService.CreateService(category);
        return Ok("Category created successfully.");
    }

    [HttpPut("update")]
    public async Task<ActionResult> UpdateCategory([FromBody] CategoryDto category)
    {
        if (category == null || category.Id <= 0)
        {
            return BadRequest("A valid Category id is required.");
        }

        var existingCategory = await _categoryService.GetByIdService(category.Id);
        if (existingCategory == null)
        {
            return NotFound($"Category with id {category.Id} was not found.");
        }

        await _categoryService.UpdateService(category);
        return Ok("Category updated successfully.");
    }

    // [HttpDelete("delete/{id}")]
    // public async Task<ActionResult> DeleteCategory(int id)
    // {
    //     var existingCategory = await _categoryService.GetByIdService(id);
    //     if (existingCategory == null)
    //     {
    //         return NotFound($"Category with id {id} was not found.");
    //     }

    //     await _categoryService.DeleteService(id);
    //     return Ok("Category deleted successfully.");
    // }
}