using Application.DTOs;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Entities;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Persistence;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly ToDoListWebContext _context;
        private readonly bool _useDirectPasswordComparison = true; // Set to true to use direct comparison instead of PasswordHasher

        public UserService(IUserRepository userRepository, ToDoListWebContext context)
        {
            _passwordHasher = new PasswordHasher<User>();
            _userRepository = userRepository;
            _context = context;
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

            // Create user
            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Store password based on configuration
            if (_useDirectPasswordComparison) {
                // Store password directly (not recommended for production)
                user.PasswordHash = userDto.Password;
            }
            else {
                // Hash the password before saving (recommended for production)
                user.PasswordHash = _passwordHasher.HashPassword(user, userDto.Password);
            }

            await _userRepository.AddAsync(user);
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return null;

            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task<bool> LoginUserAsync(string email, string password)
        {
            // Get user by email
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return false;

            // Verify password
            if (_useDirectPasswordComparison) {
                // Direct string comparison (not recommended for production)
                return user.PasswordHash == password;
            }
            else {
                try {
                    var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                    return result == PasswordVerificationResult.Success;
                } catch (FormatException) {
                    // If we get here, the password is probably not properly hashed
                    // Fall back to direct comparison as a temporary measure
                    return user.PasswordHash == password;
                }
            }
        }
    }
}