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
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ICategoryRepository _categoryRepository;

        public TaskService(ITaskRepository taskRepository, ITagRepository tagRepository, ICategoryRepository categoryRepository)
        {
            _taskRepository = taskRepository;
            _tagRepository = tagRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<bool> AddTask(TaskDTO taskdto)
        {
            var category = taskdto.CategoryId.HasValue
                ? await _categoryRepository.GetCategoryByIdAsync(taskdto.CategoryId.Value)
                : null;

            var task = new Domain.Entities.Task
            {
                UserId = taskdto.UserId,
                Title = taskdto.Title,
                Description = taskdto.Description,
                CategoryId = taskdto.CategoryId,
                Category = category,
                Status = taskdto.Status,
                Priority = taskdto.Priority,
                DueDate = taskdto.DueDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Subtasks = taskdto.Subtasks?.Select(subtask => new Subtask
                {
                    Title = subtask.Title,
                    IsCompleted = false
                }).ToList(),
                Tags = taskdto.Tags?.Select(tag => new Tag
                {
                    Name = tag.Name
                }).ToList(),
                Reminders = taskdto.Reminders?.Select(reminder => new Reminder
                {
                    ReminderTime = reminder.ReminderTime,
                    IsSent = false
                }).ToList()
            };

            int taskId = await _taskRepository.AddTaskAsync(task);
            return taskId > 0;
        }


        public async Task<List<TaskDashboard>> GetListTaskToday(int userId)
        {
            var allTasks = await _taskRepository.GetListTaskAsync(userId);

            var today = DateOnly.FromDateTime(DateTime.Today);

            var taskToday = allTasks
                .Where(t => t.DueDate.HasValue && DateOnly.FromDateTime(t.DueDate.Value) == today)
                .Select(t => new TaskDashboard
                {
                    Id = t.Id,
                    Title = t.Title,
                    Priority = t.Priority,
                    dueDate = DateOnly.FromDateTime(t.DueDate!.Value),
                    UserId = userId
                    // Add other mappings as needed
                })
                .ToList();

            return taskToday;
        }

        public async Task<List<TaskDashboard>> GetListTaskUpcoming(int userId)
        {
            var allTasks = await _taskRepository.GetListTaskAsync(userId);

            var today = DateOnly.FromDateTime(DateTime.Today);

            var upcomingTasks = allTasks
                .Where(task => task.DueDate.HasValue && DateOnly.FromDateTime(task.DueDate.Value) > today)
                .OrderBy(task => DateOnly.FromDateTime(task.DueDate.Value))
                .ToList();

            // Chuyển đổi các Task sang TaskDashboard
            var taskDashboards = upcomingTasks.Select(task => new TaskDashboard
            {
                Id = task.Id,
                Title = task.Title,
                // Kiểm tra và chuyển đổi DueDate từ DateTime? sang DateOnly
                dueDate = task.DueDate.HasValue ? DateOnly.FromDateTime(task.DueDate.Value) : default(DateOnly),
                // Chuyển thêm các thuộc tính khác nếu cần
            }).ToList();

            return taskDashboards;
        }

        public async Task<bool> UpdateTaskStatus(int taskId)
        {
            bool isAffect = await _taskRepository.UpdateTaskAsync(taskId);
            if (!isAffect) 
                return isAffect;
            
            return isAffect;
        }
        public async Task<IEnumerable<TaskDashboard>> GetCompletedTasks(int userId)
        {
            var TaskRepo = await _taskRepository.GetTaskCompletedAsync(userId);

            var TaskService = TaskRepo.Select(t => new TaskDashboard
            {
                Id = t.Id,
                Priority = t.Priority,
                Title = t.Title,
                dueDate = t.DueDate.HasValue ? DateOnly.FromDateTime(t.DueDate.Value) : default,
                UserId = userId
            }).ToList();

            return TaskService;
        }

    }
}
