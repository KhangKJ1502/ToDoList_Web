using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITaskRepository
    {
        Task<int> AddTaskAsync(Entities.Task task);
        Task<List<Entities.Task>> GetListTaskAsync(int UserId);
        Task<bool> UpdateTaskAsync(int TaskId); 
        Task<IEnumerable<Entities.Task>> GetTaskCompletedAsync(int UserId);
    }
}
