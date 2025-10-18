using System.ComponentModel.DataAnnotations;

namespace ProjectManagementTool.Models
{
    public enum ProjectRole
    {
        Owner,
        Member
    }

    public class ProjectMember
    {
        [Required]
        public int ProjectId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public ProjectRole Role { get; set; } = ProjectRole.Member;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Project Project { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
