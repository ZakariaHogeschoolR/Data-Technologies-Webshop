using DataTransferObject;
using models;

public interface IProductCategoryService
{
    Task<List<Products>> GetAllPrevService(int categoryId, int lastId);

    Task<List<Products>> GetAllNextService(int categoryId, int lastId);
}