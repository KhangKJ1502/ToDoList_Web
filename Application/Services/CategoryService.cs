using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<int> AddCategory(CategoriesDTO categoriesDTO)
        {
            // Validate input if needed
            if (string.IsNullOrEmpty(categoriesDTO.Name) || string.IsNullOrEmpty(categoriesDTO.Color)) {
                throw new ArgumentException("Category name and color are required.");
            }

            // Create category entity with all required properties
            var category = new Category
            {
                Name = categoriesDTO.Name,
                Color = categoriesDTO.Color,
                UserId = categoriesDTO.UserId, // Include UserId from DTO
                CreatedAt = DateTime.Now // Set creation date
            };

            // Add and get the ID
            return await _categoryRepository.AddAsync(category);
        }



        public async Task<List<CategoriesDTO>> GetAllCategories(int userID)
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync(userID);

            return categories.Select(c => new CategoriesDTO
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Color = c.Color,
                UserId = c.UserId
            }).ToList();
        }

        public async Task<CategoriesDTO> GetCategoriesById(int categoryId)
        {
            if (categoryId <= 0) {
                throw new ArgumentException("Mã danh mục không hợp lệ.");
            }

            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);

            if (category == null) {
                throw new KeyNotFoundException("Danh mục không tìm thấy.");
            }

            return new CategoriesDTO
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Color = category.Color,
                UserId = category.UserId
            };
        }


        public async Task<bool> UpdateCategoryById(CategoriesDTO categoryDTO)
        {
            if (categoryDTO == null) {
                throw new ArgumentNullException(nameof(categoryDTO), "Category data cannot be null.");
            }

            // Ensure category ID exists
            if (categoryDTO.CategoryId <= 0) {
                throw new ArgumentException("Invalid category ID.");
            }

            // Map CategoriesDTO to Category entity
            var category = new Category
            {
                CategoryId = categoryDTO.CategoryId, // Ensure the ID is set for update
                Name = categoryDTO.Name,
                Color = categoryDTO.Color,
                UserId = categoryDTO.UserId,
               
            };
            // Attempt to update the category
            bool isUpdated = await _categoryRepository.UpdateCategoryAstnc(category);

            return isUpdated; // Return whether the update was successful or not
        }

        public async Task<bool> DeleteCategoryById(int categoryId)
        {
            if (categoryId <= 0) {
                throw new ArgumentException("Mã danh mục không hợp lệ.");
            }
                bool isDeleted = await _categoryRepository.DeleteCategoryAsync(categoryId);
                return isDeleted;
        }
    }
}
