
using DataTransferObject;

using models;

namespace Service;

public class GraphService : IGraphService
{
    private readonly GraphRepository _graphRepository;

    public GraphService(GraphRepository GraphRepository)
    {
        _graphRepository = GraphRepository;
    }

    public async Task AddBoughtService(BoughtDto dto)
    {
        await _graphRepository.AddBought(dto);
    }

    public async Task AddBulkBoughtService(BulkBoughtDto dto) => await _graphRepository.AddBulkBought(dto);

    public async Task<List<TrendingProductDto>> TrendingTeamsService()
    {
        var Teams = await _graphRepository.TrendingProducts();
        return Teams;
    }
}
