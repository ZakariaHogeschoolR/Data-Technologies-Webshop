using ApplicationDbContext;

using DataTransferObject;

public interface IGraph
{
    Task AddBought(BoughtDto dto);

    Task AddBulkBought(BulkBoughtDto dto);

}
