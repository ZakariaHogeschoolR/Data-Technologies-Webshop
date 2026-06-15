using DataTransferObject;

using models;

public interface IProductCategory
{
    Task<List<Products>> GetAllProductCategorys(int id);

    Task<List<Products>> GetProductsPrev(int categoryId, int lastId);

    Task<List<Products>> GetProductsNext(int categoryId, int lastId);

}
