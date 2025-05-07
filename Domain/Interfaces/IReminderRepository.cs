using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IReminderRepository
    {
        Task<bool> AddReminderAsync(Reminder reminder);
    }
}
