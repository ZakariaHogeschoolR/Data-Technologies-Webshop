using ApplicationDbContext;

using DataTransferObject;

using models;

using Npgsql;

public interface ICategory
{

    Task<List<Categories>> GetAllCategories();

    Task<List<Categories>> GetAllCategoriesForGraph();

    Task<Categories?> GetCategoryById(int id);

    Task<List<Categories?>> GetCategoryByPrice(double price);

    Task<List<Categories?>> GetCategoryByTeam(string name);

    Task<List<Categories?>> GetCategorysByPrice(double price);

    Task AddCategory(CategoryDto category);

    Task UpdateCategory(CategoryDto category);

    Task DeleteCategory(int id);

}