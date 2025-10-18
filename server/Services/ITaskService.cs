using Lumo.DTOs;
using Lumo.Models;

namespace Lumo.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetTasksForProjectAsync(int projectId, int userId);
        Task<TaskDto> GetTaskByIdAsync(int taskId, int userId);
        Task<TaskDto> CreateTaskAsync(CreateTaskDto createDto, int projectId, int creatorId);
        Task<TaskDto> UpdateTaskAsync(int taskId, UpdateTaskDto updateDto, int userId);
        Task DeleteTaskAsync(int taskId, int userId);
        Task<IEnumerable<TaskDto>> UpdateTaskStatusesAsync(IEnumerable<UpdateTaskStatusDto> updates, int userId);
    }
}
