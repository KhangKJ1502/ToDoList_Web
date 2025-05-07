using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITaskService
    {
        Task<bool> AddTask(TaskDTO task);
        Task<List<TaskDashboard>> GetListTaskToday(int UserId);
        Task<List<TaskDashboard>> GetListTaskUpcoming(int UserId);
      
    }
}
