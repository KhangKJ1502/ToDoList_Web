using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<int> AddAsync(Category category);
       
        Task<List<Category>> GetAllCategoriesAsync(int UserID);  
        Task<Category>GetCategoryByIdAsync(int CategoryId);
        System.Threading.Tasks.Task<bool> UpdateCategoryAstnc(Category category);

        Task<bool> DeleteCategoryAsync(int CategoryId);
    }
}