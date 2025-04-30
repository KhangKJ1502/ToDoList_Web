using Application.DTOs;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Generators;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async  System.Threading.Tasks.Task RegisterUserAsync(UserDto userDto)
        {
            // Validate input
            if (string.IsNullOrEmpty(userDto.Username) || string.IsNullOrEmpty(userDto.Password))
                throw new ArgumentException("Username and password are required.");

            // Check if username already exists
            bool userExists = await _userRepository.ExistsAsync(userDto.Username);
            if (userExists)
                throw new InvalidOperationException("Username already taken.");

            // Proceed with registration
            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = userDto.Password,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _userRepository.AddAsync(user);
        }
        public async Task<UserDto> GetUserByNameAsync(string userName)
        {
            var user = await _userRepository.GetByUsernameAsync(userName);
            if (user == null)
                return null;

            return new UserDto
            {
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }


    }
}