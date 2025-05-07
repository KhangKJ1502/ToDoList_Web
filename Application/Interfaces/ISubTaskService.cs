using Application.DTOs;
using Infrastructure.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISubTaskService
    {
        Task<bool> AddSubTask(SubtaskDTO subtaskDto);
    }
}
