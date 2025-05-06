using Application.DTOs;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task RegisterUserAsync(UserDto userDto);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<bool> LoginUserAsync(string email, string password);
    }
}