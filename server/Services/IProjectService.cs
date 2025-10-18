using Lumo.DTOs;
using Lumo.Models;

namespace Lumo.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetProjectsForUserAsync(int userId);
        Task<ProjectDto> GetProjectByIdAsync(int projectId, int userId);
        Task<ProjectDto> CreateProjectAsync(CreateProjectDto createDto, int ownerId);
        Task<ProjectDto> UpdateProjectAsync(int projectId, UpdateProjectDto updateDto, int userId);
        Task DeleteProjectAsync(int projectId, int userId);
        Task<ProjectMemberDto> InviteMemberAsync(int projectId, InviteMemberDto inviteDto, int inviterId);
        Task<IEnumerable<ProjectMemberDto>> GetProjectMembersAsync(int projectId, int userId);
        Task<bool> IsUserProjectOwnerAsync(int projectId, int userId);
        Task<bool> IsUserProjectMemberAsync(int projectId, int userId);
    }
}
