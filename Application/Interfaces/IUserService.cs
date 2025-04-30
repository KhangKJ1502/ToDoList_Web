using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task RegisterUserAsync(UserDto userDto);
        Task<UserDto> GetUserByNameAsync(string userName);
       
    }
}