using Domain.Entities;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        System.Threading.Tasks.Task AddAsync(User user);
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByIdAsync(int userId);
        System.Threading.Tasks.Task UpdateAsync(User user);
        System.Threading.Tasks.Task DeleteAsync(int userId);
        Task<bool> ExistsAsync(string username);
    }
}