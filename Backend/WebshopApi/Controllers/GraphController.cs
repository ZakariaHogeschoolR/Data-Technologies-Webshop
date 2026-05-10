using DataTransferObject;

using Microsoft.AspNetCore.Mvc;

using models;

using Neo4j.Driver;

using Service;

[ApiController]
[Route("api/[controller]")]
public class GraphController : ControllerBase
{
    private GraphService _graphService;

    public GraphController(GraphService graphService)
    {
        _graphService = graphService;
    }

    [HttpPost("bought")]
    public async Task<IActionResult> AddBought([FromBody] BoughtDto dto)
    {
        await _graphService.AddBoughtService(dto);
        return Ok(new { message = "BOUGHT relation updated" });
    }
}
