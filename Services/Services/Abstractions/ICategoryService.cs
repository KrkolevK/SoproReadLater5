using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICategoryService
    {
        Task<Category> CreateCategory(Category category);
        Task<List<Category>> GetCategoriesByUserId(string userId);
        Category GetCategory(int Id);
        Category GetCategory(string Name);
        Task UpdateCategory(Category category);
        void DeleteCategory(Category category);
    }
}
