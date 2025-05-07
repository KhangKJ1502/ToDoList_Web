using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebUI.Commons;

namespace WebUI.Controllers.Api
{
    [ApiController]
    [Route("api/")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ITagRepository _tagRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserToolsCommon _userToolsCommon;

        public TasksController(
            ICategoryRepository categoryRepository,
            ITagRepository tagRepository,
            ITaskService taskService,
            UserToolsCommon userToolsCommon)
        {
            _taskService = taskService;
            _tagRepository = tagRepository;
            _categoryRepository = categoryRepository;
            _userToolsCommon = userToolsCommon;
        }

        [HttpPost]
        [Route("Create-task")]
        public async Task<IActionResult> CreateTask([FromBody] TaskDTO model)
        {
            int? userId = _userToolsCommon.GetIdUser();
            if (userId == null)
                return Unauthorized();

            var task = new TaskDTO
            {
                Title = model.Title,
                Description = model.Description,
                CategoryId = model.CategoryId,
                Priority = model.Priority,
                Status = model.Status,
                DueDate = model.DueDate,
                UserId = userId.Value,
                Subtasks = model.Subtasks,
                Tags = model.Tags,
                Reminders = model.Reminders
            };

            // Save the main task
            bool isCreated = await _taskService.AddTask(task);

            if (!isCreated)
                return BadRequest(new { message = "Failed to create task" });

            return Ok(new { message = "Task created successfully!" });
        }

        [HttpGet("get-infor-task")]
        public async Task<IActionResult> GetListInforTask()
        {
            int? userId = _userToolsCommon.GetIdUser();
            if (userId == null)
                return Unauthorized();

            var tags = await _tagRepository.GetAllTagAsync(userId.Value);
            var categories = await _categoryRepository.GetAllCategoriesAsync(userId.Value);

            var result = new
            {
                Tags = tags.Select(t => new
                {
                    t.TagID,
                    t.Name
                }),
                Categories = categories.Select(c => new
                {
                    c.CategoryId,
                    c.Name,
                    c.Color
                })
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("tasks/today")]
        public async Task<IActionResult> GetListTaskToday()
        {
            int? userId = _userToolsCommon?.GetIdUser();

            if (userId == null) {
                return Unauthorized();
            }

            var TaskToday = await _taskService.GetListTaskToday(userId.Value);
            if (TaskToday == null) {
                return BadRequest(new { success = false, message = "Ban khong co Note ngay hom nay" });
            }

            // Return the tasks with success status
            return Ok(new
            {
                success = true,
                data = TaskToday
            });
        } 

        [HttpGet]
        [Route("tasks/upcoming")]

        public async Task<IActionResult> GetListTaskUpcoming()
        {
            int? userId  = _userToolsCommon.GetIdUser();

            if (userId == null) {
                return Unauthorized();
            }
            var TaskUpComing = await _taskService.GetListTaskUpcoming(userId.Value);
            if (TaskUpComing == null) {
                return BadRequest(new { success = false, message = "Ban khong co Note ngay hom nay" });
            }
            return Ok(new
            {
                success = true,
                data = TaskUpComing
            });


        }


    }
}
