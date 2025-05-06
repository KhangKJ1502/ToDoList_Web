using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Category = Domain.Entities.Category;

namespace Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ToDoListWebContext _context;

        public CategoryRepository(ToDoListWebContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(Category category)
        {
            // Map from Domain.Entities.Category to Infrastructure.Persistence.Models.Category
            var infrastructureCategory = new Infrastructure.Persistence.Models.Category
            {
                Name = category.Name,
                Color = category.Color,
                CreatedAt = category.CreatedAt,
                UserId = category.UserId
            };

            await _context.Categories.AddAsync(infrastructureCategory);
            await _context.SaveChangesAsync();

            return infrastructureCategory.CategoryId;
        }

        public async Task<List<Category>> GetAllCategoriesAsync(int userID)
        {
            // Fetch data from DB
            var infrastructureCategories = await _context.Categories
                .Where(c => c.UserId == userID)
                .ToListAsync();

            // Map from Infrastructure.Persistence.Models.Category to Domain.Entities.Category
            return infrastructureCategories.Select(ic => new Category
            {
                CategoryId = ic.CategoryId,
                Name = ic.Name,
                Color = ic.Color,
                UserId = ic.UserId,
                CreatedAt = ic.CreatedAt
            }).ToList();
        }

        public async Task<Domain.Entities.Category> GetCategoryByIdAsync(int categoryId)
        {
            var existing = await _context.Categories.FindAsync(categoryId);

            if (existing == null)
                throw new KeyNotFoundException("Danh mục không tìm thấy.");

            return new Domain.Entities.Category
            {
                CategoryId = existing.CategoryId,
                Name = existing.Name,
                Color = existing.Color,
                UserId = existing.UserId,
                CreatedAt = existing.CreatedAt
            };
        }


        public async Task<bool> UpdateCategoryAstnc(Category category)
        {
            var existingCategory = await _context.Categories.FindAsync(category.CategoryId);

            if (existingCategory == null) {
                // You might want to throw a more specific exception or return false.
                throw new KeyNotFoundException("Danh mục không tìm thấy.");
            }

            // Update properties
            existingCategory.Name = category.Name;
            existingCategory.Color = category.Color;

            _context.Categories.Update(existingCategory);

            try {
                // Save changes and check if any rows were affected
                int affectedRows = await _context.SaveChangesAsync();

                return affectedRows > 0; // Return true if update was successful
            } catch (Exception ex) {
                // Log or handle the exception if needed
                // Rethrow or handle accordingly
                throw new InvalidOperationException("Failed to update category.", ex);
            }
        }

        public async Task<bool> DeleteCategoryAsync(int CategoryId)
        {
            var existingCategories = await _context.Categories.FindAsync(CategoryId);
            if (existingCategories == null) {
                throw new ArgumentException("Mã danh mục không tồn tại.");
            }
            _context.Categories.Remove(existingCategories);
            try { 
                int affectedRows = await _context.SaveChangesAsync();
            return affectedRows > 0;
            } catch (Exception ex) {
                throw new InvalidOperationException("Xóa dữ liệu không thành công.");
            }
        }
        
        
        


    }
 }
