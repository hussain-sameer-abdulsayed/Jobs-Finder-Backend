using MB_Project.Models;
using MB_Project.Models.DTOS.CategoryDto;

namespace MB_Project.IRepos
{
    public interface ICategoryRepo
    {
        Task<IEnumerable<Category>> GetAllCategories();
        Task<Category> GetCategoryById(int id);
        Task<List<Category>> GetCategoryByName(string name);
        Task<bool> CreateCategory(Category category);
        Task<bool> UpdateCategory(int id,Category category);
        Task<bool> DeleteCategoryById(int id);
    }
}
