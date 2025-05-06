using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers.Api
{
    [ApiController]
    [Route("api/")]
    public class TodoController : ControllerBase
    {
        private readonly ICategoryService _categoryService; 
        
        public TodoController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> CreateTask()
        {
            return Ok(new {success = true , message = ""});
        }
    }
}
