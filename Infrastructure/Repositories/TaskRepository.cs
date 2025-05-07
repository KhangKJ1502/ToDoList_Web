using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ToDoListWebContext _context;
        private readonly ISubTaskRepository _subtaskRepository;
        private readonly IReminderRepository _reminderRepository;

        public TaskRepository(
            ToDoListWebContext context,
            ISubTaskRepository subtaskRepository,
            IReminderRepository reminderRepository)
        {
            _context = context;
            _subtaskRepository = subtaskRepository;
            _reminderRepository = reminderRepository;
        }

        public async Task<int> AddTaskAsync(Domain.Entities.Task task)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try {
                var taskEntity = new Infrastructure.Persistence.Models.Task
                {
                    UserId = task.UserId,
                    Title = task.Title,
                    Description = task.Description,
                    CategoryId = task.CategoryId,
                    Status = task.Status,
                    Priority = task.Priority,
                    DueDate = task.DueDate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.Tasks.AddAsync(taskEntity);
                await _context.SaveChangesAsync();

                // Handle Subtasks
                if (task.Subtasks?.Any() == true) {
                    var subtaskEntities = task.Subtasks.Select(subtask => new Subtask
                    {
                        TaskId = taskEntity.TaskId,
                        Title = subtask.Title,
                        IsCompleted = subtask.IsCompleted,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });

                    await _context.Subtasks.AddRangeAsync(subtaskEntities);
                }

                // Handle Tags - Fixed approach using TaskTag entity
                if (task.Tags?.Any() == true) {
                    foreach (var tag in task.Tags) {
                        // Normalize tag name (trim and lowercase)
                        var normalizedTagName = tag.Name?.Trim().ToLower();

                        if (string.IsNullOrEmpty(normalizedTagName))
                            continue;

                        var existingTag = await _context.Tags
                            .FirstOrDefaultAsync(t => t.Name == normalizedTagName && t.UserId == task.UserId);

                        if (existingTag == null) {
                            existingTag = new Tag
                            {
                                UserId = task.UserId,
                                Name = normalizedTagName
                            };
                            await _context.Tags.AddAsync(existingTag);
                            await _context.SaveChangesAsync(); // Save to get the TagId
                        }

                        // Create and add TaskTag entity instead of using raw SQL
                        var taskTag = new TaskTag
                        {
                            TaskId = taskEntity.TaskId,
                            TagId = existingTag.TagId,
                            Task = taskEntity,
                            Tag = existingTag
                        };

                        // Check if the relationship already exists
                        bool exists = await _context.Set<TaskTag>()
                            .AnyAsync(tt => tt.TaskId == taskEntity.TaskId && tt.TagId == existingTag.TagId);

                        if (!exists) {
                            _context.Set<TaskTag>().Add(taskTag);
                        }
                    }
                }

                // Handle Reminders
                if (task.Reminders?.Any() == true) {
                    var reminderEntities = task.Reminders.Select(reminder => new Reminder
                    {
                        TaskId = taskEntity.TaskId,
                        ReminderTime = reminder.ReminderTime,
                        IsSent = false,
                        CreatedAt = DateTime.UtcNow
                    });

                    await _context.Reminders.AddRangeAsync(reminderEntities);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return taskEntity.TaskId;
            } catch (Exception ex) {
                await transaction.RollbackAsync();
                // Log the exception here
                throw; // Re-throw the exception after rollback
            }
        }

        public async Task<List<Domain.Entities.Task>> GetListTaskAsync(int userId)
        {
            var dbTasks = await _context.Tasks
                .Where(t => t.UserId == userId)
                .ToListAsync();

            var domainTasks = dbTasks.Select(t => new Domain.Entities.Task
            {
                Id = t.TaskId,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority,
                DueDate = t.DueDate,
                UserId = t.UserId,
                // map other properties
            }).ToList();

            return domainTasks;
        }

    }
}