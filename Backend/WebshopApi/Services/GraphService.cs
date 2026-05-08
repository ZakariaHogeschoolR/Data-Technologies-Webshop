
using DataTransferObject;

using models;

namespace Service;

public class GraphService
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
}