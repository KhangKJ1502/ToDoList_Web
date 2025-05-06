using Application.DTOs;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        Task<int> AddCategory(CategoriesDTO categoriesDTO);

        Task<List<CategoriesDTO>> GetAllCategories(int UserID);

        Task<CategoriesDTO> GetCategoriesById(int categoryId);

        Task<bool> UpdateCategoryById(CategoriesDTO categoryDTO);

        Task<bool> DeleteCategoryById(int categoryId);
    }
}