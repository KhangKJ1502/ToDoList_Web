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
    public class ReminderRepository : IReminderRepository
    {
        private readonly ToDoListWebContext _context;
        public ReminderRepository(ToDoListWebContext context)
        {
            _context = context;
        }
        public async Task<bool> AddReminderAsync(Domain.Entities.Reminder reminder)
        {
            var reminderEntity = new Infrastructure.Persistence.Models.Reminder
            {
                TaskId = reminder.TaskID,
                ReminderTime = reminder.ReminderTime,
                IsSent = reminder.IsSent
            };

            await _context.Reminders.AddAsync(reminderEntity);
            var result = await _context.SaveChangesAsync(); // tra ve so dong anh huong

            return result > 0;
        }

    }
}
