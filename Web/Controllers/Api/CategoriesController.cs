using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebUI.Commons;

namespace WebUI.Controllers.Api
{
    [ApiController]
    [Route("api/")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService; 
        private readonly UserToolsCommon _tools;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, UserToolsCommon Tools, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _tools = Tools;
            _logger = logger;
        }

        [Route("add-Categories")]
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoriesDTO categoriesDTO)
        {
         int? userId = _tools.GetIdUser();
            if (userId == null) {
                return BadRequest(new { success = false, message = "Invalid UserId in claims" });
            } 
            categoriesDTO.UserId = (int)userId;

            if (string.IsNullOrEmpty(categoriesDTO.Name) || string.IsNullOrEmpty(categoriesDTO.Color)) {
                return BadRequest(new { success = false, message = "Name and color cannot be empty" });
            }

            try {
                int categoryId = await _categoryService.AddCategory(categoriesDTO);

                return Ok(new
                {
                    success = true,
                    message = "Category added successfully",
                    categoryId = categoryId
                });
            } catch (System.Exception ex) {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [Route("list-Category")]
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try { 
                // Chuyển đổi từ string sang int
                int? userId = _tools.GetIdUser() ;
                if (userId == null) {
                    return BadRequest(new { success = false, message = "Invalid UserId in claims" });
                }
                _logger.LogInformation("UserId: {UserId}", userId);

                // Lấy tất cả các categories cho UserId
                var categories = await _categoryService.GetAllCategories((int)userId);

                return Ok(new
                {
                    success = true,
                    data = categories,
                    message = "Categories retrieved successfully"
                });
            } catch (System.Exception ex) {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Failed to retrieve categories",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        [HttpGet("get-category-by-id")]
        public async Task<IActionResult> GetCategoryById([FromQuery] int categoryId)
        {
            _logger.LogInformation($"Thong tin Get Category: {categoryId}");
            if (categoryId == 0) {
                return BadRequest(new
                {
                    success = false,
                    message = "Mã danh mục không tồn tại."
                });
            }
               try{ 
                   CategoriesDTO categories = await _categoryService.GetCategoriesById(categoryId);

                    if (categories == null) { 
                    return NotFound();
} 
                    return Ok(new { success = true, data = categories });

                }catch (System.Exception ex) {
                    return StatusCode(500, new
                    {
                        success = false,
                        message = "Failed to get categories by Id",
                        error = ex.Message
                    });
                }

            }

        [HttpPut]
        [Route("update-categories")]
        public async Task<IActionResult> UpdateCategories([FromBody] CategoriesDTO categoriesDTO)
        {
            if (categoriesDTO == null) {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid category data provided."
                });
            }

            try {
                // Get the userId from the service
                int? userId = _tools.GetIdUser();

                if (userId == null) {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User not authenticated."
                    });
                }

                categoriesDTO.UserId = userId.Value; // Safe to access userId here

                // Update category via service
                bool isUpdated = await _categoryService.UpdateCategoryById(categoriesDTO);

                if (!isUpdated) {
                    return NotFound(new
                    {
                        success = false,
                        message = "Category not found or update failed."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Category updated successfully."
                });
            } catch (System.Exception ex) {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Failed to update categories",
                    error = ex.Message
                });
            }
        }

        [HttpDelete]
        [Route("delete-categories-by-id")] 
        public async Task<IActionResult> DeleteCategoriesById([FromQuery]int categoryId)
        {
            if (categoryId <= 0) {
                return BadRequest(new { success = false, message = "Mã danh mục không hợp lệ." });
            }
            try {
                bool isDeleted = await _categoryService.DeleteCategoryById(categoryId);
                if (isDeleted) {
                    return Ok(new { success = true, message = "Danh mục đã được xóa thành công." });
                }
                return BadRequest(new {success = false, message = "Danh mục chưa được xóa."});

            } catch (System.Exception ex) {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Danh mục xóa thất bại.",
                    error = ex.Message
                });
            }
         
          
        }




    }
}