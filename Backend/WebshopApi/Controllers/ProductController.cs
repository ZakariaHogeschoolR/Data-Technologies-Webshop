using Microsoft.AspNetCore.Mvc;
using models;
using Service;

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

    [HttpGet("/id")]
    public async Task<ActionResult<Products>> GetAllUsersById(int id)
    {
        var product = _productService.GetByIdService(id);
        return Ok(product);
    }

    [HttpPost("/create")]
    public async Task<ActionResult> CreateProduct(Products product)
    {
        _productService.CreateService(product);
        return Ok();
    }

    [HttpPost("/Update")]
    public async Task<ActionResult> UpdateProduct(Products product)
    {
        _productService.UpdateService(product);
        return Ok();
    }

    [HttpDelete("/Delete")]
    public async Task<ActionResult> DeleteUser(Products product)
    {
        _productService.DeleteService(product.Id);
        return Ok();
    }
}