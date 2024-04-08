using MB_Project.Data;
using MB_Project.IRepos;
using MB_Project.Models;
using MB_Project.Models.DTOS.CategoryDto;
using Microsoft.EntityFrameworkCore;

namespace MB_Project.Repos
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly MB_ProjectContext _context;
        public CategoryRepo(MB_ProjectContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateCategory(Category category)
        {
            try
            {
                await _context.Categories.AddAsync(category);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> DeleteCategoryById(int id)
        {
            try
            {
                var obj = await _context.Categories.FindAsync(id);
                if (obj == null)
                {
                    return false;
                }
                _context.Attach(obj);
                _context.Categories.Remove(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            try
            {
                var obj = await _context.Categories
                                        .Select(c=> new Category
                                        {
                                            Id = c.Id,
                                            Name = c.Name,
                                            ShowOnMain = c.ShowOnMain
                                        })
                                        .ToListAsync();
                return (IEnumerable<Category >)obj;
            }
            catch
            {
                return Enumerable.Empty<Category>();
            }
        }
        public async Task<Category> GetCategoryById(int id)
        {
            try
            {
                var obj = await _context.Categories.Where(x => x.Id == id)
                    .Select(c => new Category
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ShowOnMain = c.ShowOnMain,
                        Posts = c.Posts.Select(p => new Post
                        {
                            Id = p.Id,
                            Title = p.Title,
                            BasePrice = p.BasePrice,
                            MainImage = p.MainImage
                        }).ToList()
                    }).FirstOrDefaultAsync();
                return obj;
            }
            catch
            {
                return null;
            }
        }
        public async Task<List<Category>> GetCategoryByName(string name)
        {
            try
            {
                var obj = await _context.Categories.Where(x=>EF.Functions.Like(x.Name,$"%{name}%"))
                                                    .Select(c=> new Category
                                                    {
                                                        Id= c.Id,
                                                        Name = c.Name,
                                                        ShowOnMain = c.ShowOnMain
                                                    }).ToListAsync();
                return obj;
            }
            catch
            {
                return null;
            }
        }
        public async Task<bool> UpdateCategory(int id, Category category)
        {
            try
            {
                var obj = await _context.Categories.FindAsync(id);
                if (obj == null)
                {
                    return false;
                }
                _context.Attach(obj);
                obj.Name = category.Name;
                obj.ShowOnMain = category.ShowOnMain;
                await _context.SaveChangesAsync();
                return true;
            }
            catch 
            { 
                return false;
            }
        }
    }
}
