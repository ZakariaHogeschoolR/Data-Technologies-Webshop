using ApplicationDbContext;
using DataTransferObject;

public interface IGraphService
{
    Task AddBoughtService(BoughtDto dto);
}