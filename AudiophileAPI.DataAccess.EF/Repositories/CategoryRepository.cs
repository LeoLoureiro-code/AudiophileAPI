using AudiophileAPI.DataAccess.EF.Context;
using AudiophileAPI.DataAccess.EF.Interfaces;
using AudiophileAPI.DataAccess.EF.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiophileAPI.DataAccess.EF.Repositories
{
    public class CategoryRepository: ICategoryRepository
    {
        private readonly AudiophileAPIDbContext _context;

        public CategoryRepository(AudiophileAPIDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryById(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                throw new Exception("Category not found");
            }
            return category;
        }

        public async Task<Category> AddCategory(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategory(int categoryId, string categoryName)
        {
            Category existingCategory = await _context.Categories.FindAsync(categoryId);

            if (existingCategory == null)
            {
                throw new Exception("Category not found");
            }

            existingCategory.CategoryName = categoryName;
            await _context.SaveChangesAsync();

            return existingCategory;
        }

        public async Task DeleteCategory(int id)
        {
            Category category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                throw new Exception("Category not found");
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
