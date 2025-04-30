using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ToDoListWebContext _context;

        public UserRepository(ToDoListWebContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task AddAsync(User user)
        {
            var infrastructureUser = new Infrastructure.Persistence.Models.User
            {
                Username = user.Username,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
            Console.WriteLine(infrastructureUser.Email);

            await _context.Users.AddAsync(infrastructureUser);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            var infrastructureUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (infrastructureUser == null)
                return null;

            return new User
            {
                UserId = infrastructureUser.UserId,
                Username = infrastructureUser.Username,
                Email = infrastructureUser.Email,
                PasswordHash = infrastructureUser.PasswordHash,
                FirstName = infrastructureUser.FirstName,
                LastName = infrastructureUser.LastName,
                CreatedAt = infrastructureUser.CreatedAt,
                UpdatedAt = infrastructureUser.UpdatedAt
            };
        }

        public async Task<User> GetByIdAsync(int userId)
        {
            var infrastructureUser = await _context.Users.FindAsync(userId);
            if (infrastructureUser == null)
                return null;

            return new User
            {
                UserId = infrastructureUser.UserId,
                Username = infrastructureUser.Username,
                Email = infrastructureUser.Email,
                PasswordHash = infrastructureUser.PasswordHash,
                FirstName = infrastructureUser.FirstName,
                LastName = infrastructureUser.LastName,
                CreatedAt = infrastructureUser.CreatedAt,
                UpdatedAt = infrastructureUser.UpdatedAt
            };
        }

        public async System.Threading.Tasks.Task UpdateAsync(User user)
        {
            var infrastructureUser = await _context.Users.FindAsync(user.UserId);
            if (infrastructureUser == null)
                return;

            infrastructureUser.Username = user.Username;
            infrastructureUser.Email = user.Email;
            infrastructureUser.FirstName = user.FirstName;
            infrastructureUser.LastName = user.LastName;
            infrastructureUser.UpdatedAt = DateTime.Now;

            _context.Users.Update(infrastructureUser);
            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task DeleteAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null) {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        // Remove the unused NameAsync method
    }
}