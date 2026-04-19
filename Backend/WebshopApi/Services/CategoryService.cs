using DataTransferObject;

using models;

namespace Service;

public class CategoryService
{
    private readonly CategoryRepository _categoryRepository;

    public CategoryService(CategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<Categories>> GetAllService()
    {
        Task<List<Categories?>> category = _categoryRepository.GetAllCategories();
        return await category;
    }

    public async Task<Categories> GetByIdService(int id)
    {
        Task<Categories?> category = _categoryRepository.GetCategoryById(id);
        return await category;
    }

    public async Task<List<Categories>> GetByPriceService(double price)
    {
        Task<List<Categories?>> category = _categoryRepository.GetCategoryByPrice(price);
        return await category;
    }

    public async Task<List<Categories>> GetByNameService(string name)
    {
        Task<List<Categories?>> category = _categoryRepository.GetCategoryByTeam(name);
        return await category;
    }

    public async Task CreateService(CategoryDto category)
    {
        await _categoryRepository.AddCategory(category);
    }

    public async Task UpdateService(CategoryDto category)
    {
        await _categoryRepository.UpdateCategory(category);
    }

    public async Task DeleteService(int id)
    {
        await _categoryRepository.DeleteCategory(id);
    }
}
