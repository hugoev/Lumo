using Lumo.DTOs;
using Lumo.Models;

namespace Lumo.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm);
    }
}
