using System.ComponentModel.DataAnnotations;
using ProjectManagementTool.Models;

namespace ProjectManagementTool.DTOs
{
    public class InviteMemberDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class ProjectMemberDto
    {
        public UserDto User { get; set; } = null!;
        public ProjectRole Role { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
