using ProjectManagementTool.DTOs;
using ProjectManagementTool.Models;

namespace ProjectManagementTool.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm);
    }
}
