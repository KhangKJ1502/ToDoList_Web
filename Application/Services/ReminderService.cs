using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ReminderService : IReminderService
    {
        private readonly IReminderRepository _repository;

        public ReminderService(ReminderRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> AddReminder(ReminderDTO reminderDTO)
        {
            var Remider = new Reminder
            {
                TaskID = reminderDTO.TaskID,
                ReminderTime = reminderDTO.ReminderTime,
                IsSent = reminderDTO.IsSent
            };

            return await _repository.AddReminderAsync(Remider);
        }
    }
}
