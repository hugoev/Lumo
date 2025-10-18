using Microsoft.EntityFrameworkCore;
using ProjectManagementTool.Data;
using ProjectManagementTool.DTOs;
using ProjectManagementTool.Models;

namespace ProjectManagementTool.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly IProjectService _projectService;

        public TaskService(ApplicationDbContext context, IProjectService projectService)
        {
            _context = context;
            _projectService = projectService;
        }

        public async Task<IEnumerable<TaskDto>> GetTasksForProjectAsync(int projectId, int userId)
        {
            var isMember = await _projectService.IsUserProjectMemberAsync(projectId, userId);
            if (!isMember)
            {
                throw new UnauthorizedAccessException("Access denied");
            }

            var tasks = await _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.Assignee)
                .OrderBy(t => t.Status)
                .ThenBy(t => t.Order)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    ProjectId = t.ProjectId,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    Assignee = t.Assignee != null ? new UserDto
                    {
                        Id = t.Assignee.Id,
                        FullName = t.Assignee.FullName,
                        Email = t.Assignee.Email,
                        CreatedAt = t.Assignee.CreatedAt
                    } : null,
                    DueDate = t.DueDate,
                    Order = t.Order,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .ToListAsync();

            return tasks;
        }

        public async Task<TaskDto> GetTaskByIdAsync(int taskId, int userId)
        {
            var task = await _context.Tasks
                .Where(t => t.Id == taskId)
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    ProjectId = t.ProjectId,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    Assignee = t.Assignee != null ? new UserDto
                    {
                        Id = t.Assignee.Id,
                        FullName = t.Assignee.FullName,
                        Email = t.Assignee.Email,
                        CreatedAt = t.Assignee.CreatedAt
                    } : null,
                    DueDate = t.DueDate,
                    Order = t.Order,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (task == null)
            {
                throw new KeyNotFoundException("Task not found");
            }

            // Check if user has access to the project
            var isMember = await _projectService.IsUserProjectMemberAsync(task.ProjectId, userId);
            if (!isMember)
            {
                throw new UnauthorizedAccessException("Access denied");
            }

            return task;
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto createDto, int projectId, int creatorId)
        {
            var isMember = await _projectService.IsUserProjectMemberAsync(projectId, creatorId);
            if (!isMember)
            {
                throw new UnauthorizedAccessException("Access denied");
            }

            // Get the next order number for the status
            var maxOrder = await _context.Tasks
                .Where(t => t.ProjectId == projectId && t.Status == createDto.Status)
                .MaxAsync(t => (int?)t.Order) ?? 0;

            var task = new ProjectTask
            {
                ProjectId = projectId,
                Title = createDto.Title,
                Description = createDto.Description,
                Status = createDto.Status,
                AssigneeId = createDto.AssigneeId,
                DueDate = createDto.DueDate,
                Order = createDto.Order > 0 ? createDto.Order : maxOrder + 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return await GetTaskByIdAsync(task.Id, creatorId);
        }

        public async Task<TaskDto> UpdateTaskAsync(int taskId, UpdateTaskDto updateDto, int userId)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
            {
                throw new KeyNotFoundException("Task not found");
            }

            var isMember = await _projectService.IsUserProjectMemberAsync(task.ProjectId, userId);
            if (!isMember)
            {
                throw new UnauthorizedAccessException("Access denied");
            }

            // Check if user can edit this task (owner, assignee, or project owner)
            var isProjectOwner = await _projectService.IsUserProjectOwnerAsync(task.ProjectId, userId);
            var isAssignee = task.AssigneeId == userId;

            if (!isProjectOwner && !isAssignee)
            {
                throw new UnauthorizedAccessException("You can only edit tasks assigned to you or if you're the project owner");
            }

            if (updateDto.Title != null)
                task.Title = updateDto.Title;

            if (updateDto.Description != null)
                task.Description = updateDto.Description;

            if (updateDto.AssigneeId != null)
                task.AssigneeId = updateDto.AssigneeId;

            if (updateDto.DueDate != null)
                task.DueDate = updateDto.DueDate;

            if (updateDto.Status != null)
                task.Status = updateDto.Status.Value;

            if (updateDto.Order != null)
                task.Order = updateDto.Order.Value;

            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetTaskByIdAsync(taskId, userId);
        }

        public async Task DeleteTaskAsync(int taskId, int userId)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
            {
                throw new KeyNotFoundException("Task not found");
            }

            var isProjectOwner = await _projectService.IsUserProjectOwnerAsync(task.ProjectId, userId);
            if (!isProjectOwner)
            {
                throw new UnauthorizedAccessException("Only project owners can delete tasks");
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TaskDto>> UpdateTaskStatusesAsync(IEnumerable<UpdateTaskStatusDto> updates, int userId)
        {
            var updatedTasks = new List<TaskDto>();

            foreach (var update in updates)
            {
                var task = await _context.Tasks.FindAsync(update.TaskId);
                if (task == null) continue;

                var isMember = await _projectService.IsUserProjectMemberAsync(task.ProjectId, userId);
                if (!isMember) continue;

                task.Status = update.Status;
                task.Order = update.Order;
                task.UpdatedAt = DateTime.UtcNow;

                var taskDto = await GetTaskByIdAsync(task.Id, userId);
                updatedTasks.Add(taskDto);
            }

            await _context.SaveChangesAsync();
            return updatedTasks;
        }
    }
}
