using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class SubTaskRepository : ISubTaskRepository
    {
        private readonly ToDoListWebContext _context;

        public SubTaskRepository(ToDoListWebContext context)
        {
            _context = context;
        }


        public async Task<bool> AddSubTaskAsync(Domain.Entities.Subtask subtask)
        {
            var subtaskEntity = new Persistence.Models.Subtask
            {
                TaskId = subtask.TaskID,
                Title = subtask.Title,
                IsCompleted = subtask.IsCompleted
            };

            await _context.Subtasks.AddAsync(subtaskEntity);
            var result = await _context.SaveChangesAsync();

            return result > 0;
        }
    }
}
