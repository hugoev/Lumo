using System.ComponentModel.DataAnnotations;

namespace Lumo.Models
{
    public enum ProjectTaskStatus
    {
        Todo,
        InProgress,
        Done
    }

    public class ProjectTask
    {
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public ProjectTaskStatus Status { get; set; } = ProjectTaskStatus.Todo;

        public int? AssigneeId { get; set; }

        public DateTime? DueDate { get; set; }

        public int Order { get; set; } = 0; // For drag-and-drop ordering

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Project Project { get; set; } = null!;
        public User? Assignee { get; set; }
    }
}
