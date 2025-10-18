using System.ComponentModel.DataAnnotations;

namespace Lumo.DTOs
{
    public class CreateProjectDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateProjectDto
    {
        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }
    }

    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public UserDto Owner { get; set; } = null!;
        public List<UserDto> Members { get; set; } = new();
        public int TaskCount { get; set; }
    }
}
