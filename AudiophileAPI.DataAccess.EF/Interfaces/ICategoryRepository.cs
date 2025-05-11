using AudiophileAPI.DataAccess.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiophileAPI.DataAccess.EF.Interfaces
{
   public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategories();

        Task<Category> GetCategoryById(int id);

        Task<Category> AddCategory(Category category);

        Task<Category> UpdateCategory(int categoryId, string categoryName);

        Task DeleteCategory(int id);
    }
}
