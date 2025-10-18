using System.ComponentModel.DataAnnotations;
using ProjectManagementTool.Models;

namespace ProjectManagementTool.DTOs
{
    public class CreateTaskDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public int? AssigneeId { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskStatus Status { get; set; } = TaskStatus.Todo;

        public int Order { get; set; } = 0;
    }

    public class UpdateTaskDto
    {
        [StringLength(200)]
        public string? Title { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public int? AssigneeId { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskStatus? Status { get; set; }

        public int? Order { get; set; }
    }

    public class TaskDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskStatus Status { get; set; }
        public UserDto? Assignee { get; set; }
        public DateTime? DueDate { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UpdateTaskStatusDto
    {
        [Required]
        public int TaskId { get; set; }

        [Required]
        public TaskStatus Status { get; set; }

        public int Order { get; set; }
    }
}
