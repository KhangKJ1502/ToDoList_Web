using Application.DTOs;
using Application.Interfaces;
using Domain.Entities; // Make sure you're using the correct namespace for Subtask entity
using Domain.Interfaces;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SubTaskService : ISubTaskService
    {
        private readonly ISubTaskRepository _repository;

        public SubTaskService(ISubTaskRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> AddSubTask(SubtaskDTO subtaskDto)
        {
            var subtask = new Subtask
            {
                Title = subtaskDto.Title,
                TaskID = subtaskDto.TaskID,
                IsCompleted = subtaskDto.IsCompleted
            };

            return await _repository.AddSubTaskAsync(subtask);
        }
    }
}
