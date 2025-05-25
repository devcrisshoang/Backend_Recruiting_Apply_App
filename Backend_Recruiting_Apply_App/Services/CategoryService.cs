using Microsoft.EntityFrameworkCore;
using Backend_Recruiting_Apply_App.Data.Entities;
using SystemAPIdotnet.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Recruiting_Apply_App.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<List<Category>> GetCategoriesByResumeIdAsync(int resumeId);
        Task<Category> CreateCategoryAsync(Category category);
        Task<bool> UpdateCategoryAsync(int id, Category category);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> DeleteCategoriesByResumeIdAsync(int resumeId);
        Task<bool> DeleteAllCategoriesAsync();
    }

    public class CategoryService : ICategoryService
    {
        private readonly RAADbContext _context;

        public CategoryService(RAADbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Category.ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Category.FindAsync(id);
        }

        public async Task<List<Category>> GetCategoriesByResumeIdAsync(int resumeId)
        {
            return await _context.Category
                .Where(a => a.Resume_ID == resumeId)
                .ToListAsync();
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            _context.Category.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> UpdateCategoryAsync(int id, Category category)
        {
            if (id != category.ID)
            {
                return false;
            }

            var existingCategory = await _context.Category.FindAsync(id);
            if (existingCategory == null)
            {
                return false;
            }

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.Resume_ID = category.Resume_ID;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return !await _context.Category.AnyAsync(e => e.ID == id);
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return false;
            }

            _context.Category.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategoriesByResumeIdAsync(int resumeId)
        {
            var categories = await _context.Category
                .Where(c => c.Resume_ID == resumeId)
                .ToListAsync();

            if (categories == null || !categories.Any())
            {
                return false;
            }

            _context.Category.RemoveRange(categories);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAllCategoriesAsync()
        {
            var categories = await _context.Category.ToListAsync();
            if (categories == null || !categories.Any())
            {
                return false;
            }

            _context.Category.RemoveRange(categories);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}