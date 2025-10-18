using Microsoft.EntityFrameworkCore;
using ProjectManagementTool.Data;
using ProjectManagementTool.DTOs;
using ProjectManagementTool.Models;

namespace ProjectManagementTool.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public ProjectService(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<IEnumerable<ProjectDto>> GetProjectsForUserAsync(int userId)
        {
            var projects = await _context.Projects
                .Where(p => p.OwnerId == userId ||
                           p.Members.Any(m => m.UserId == userId))
                .Include(p => p.Owner)
                .Include(p => p.Members)
                    .ThenInclude(m => m.User)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    Owner = new UserDto
                    {
                        Id = p.Owner.Id,
                        FullName = p.Owner.FullName,
                        Email = p.Owner.Email,
                        CreatedAt = p.Owner.CreatedAt
                    },
                    Members = p.Members.Select(m => new UserDto
                    {
                        Id = m.User.Id,
                        FullName = m.User.FullName,
                        Email = m.User.Email,
                        CreatedAt = m.User.CreatedAt
                    }).ToList(),
                    TaskCount = p.Tasks.Count
                })
                .ToListAsync();

            return projects;
        }

        public async Task<ProjectDto> GetProjectByIdAsync(int projectId, int userId)
        {
            var project = await _context.Projects
                .Where(p => p.Id == projectId && (p.OwnerId == userId || p.Members.Any(m => m.UserId == userId)))
                .Include(p => p.Owner)
                .Include(p => p.Members)
                    .ThenInclude(m => m.User)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    Owner = new UserDto
                    {
                        Id = p.Owner.Id,
                        FullName = p.Owner.FullName,
                        Email = p.Owner.Email,
                        CreatedAt = p.Owner.CreatedAt
                    },
                    Members = p.Members.Select(m => new UserDto
                    {
                        Id = m.User.Id,
                        FullName = m.User.FullName,
                        Email = m.User.Email,
                        CreatedAt = m.User.CreatedAt
                    }).ToList(),
                    TaskCount = p.Tasks.Count
                })
                .FirstOrDefaultAsync();

            if (project == null)
            {
                throw new UnauthorizedAccessException("Project not found or access denied");
            }

            return project;
        }

        public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto createDto, int ownerId)
        {
            var project = new Project
            {
                Name = createDto.Name,
                Description = createDto.Description,
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Projects.Add(project);

            // Add owner as a member with Owner role
            var membership = new ProjectMember
            {
                ProjectId = project.Id,
                UserId = ownerId,
                Role = ProjectRole.Owner,
                JoinedAt = DateTime.UtcNow
            };
            _context.ProjectMembers.Add(membership);

            await _context.SaveChangesAsync();

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                Owner = await _context.Users
                    .Where(u => u.Id == ownerId)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        FullName = u.FullName,
                        Email = u.Email,
                        CreatedAt = u.CreatedAt
                    })
                    .FirstAsync(),
                Members = new List<UserDto>(),
                TaskCount = 0
            };
        }

        public async Task<ProjectDto> UpdateProjectAsync(int projectId, UpdateProjectDto updateDto, int userId)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.OwnerId == userId);

            if (project == null)
            {
                throw new UnauthorizedAccessException("Project not found or access denied");
            }

            if (updateDto.Name != null)
                project.Name = updateDto.Name;

            if (updateDto.Description != null)
                project.Description = updateDto.Description;

            await _context.SaveChangesAsync();

            return await GetProjectByIdAsync(projectId, userId);
        }

        public async Task DeleteProjectAsync(int projectId, int userId)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.OwnerId == userId);

            if (project == null)
            {
                throw new UnauthorizedAccessException("Project not found or access denied");
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }

        public async Task<ProjectMemberDto> InviteMemberAsync(int projectId, InviteMemberDto inviteDto, int inviterId)
        {
            // Check if inviter is project owner or member
            var isAuthorized = await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == projectId &&
                               pm.UserId == inviterId &&
                               (pm.Role == ProjectRole.Owner || pm.Role == ProjectRole.Member));

            if (!isAuthorized)
            {
                throw new UnauthorizedAccessException("Not authorized to invite members");
            }

            // Find user to invite
            var userToInvite = await _userService.GetUserByEmailAsync(inviteDto.Email);
            if (userToInvite == null)
            {
                throw new InvalidOperationException("User not found");
            }

            // Check if already a member
            var existingMembership = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userToInvite.Id);

            if (existingMembership != null)
            {
                throw new InvalidOperationException("User is already a member of this project");
            }

            // Add as member
            var membership = new ProjectMember
            {
                ProjectId = projectId,
                UserId = userToInvite.Id,
                Role = ProjectRole.Member,
                JoinedAt = DateTime.UtcNow
            };

            _context.ProjectMembers.Add(membership);
            await _context.SaveChangesAsync();

            return new ProjectMemberDto
            {
                User = new UserDto
                {
                    Id = userToInvite.Id,
                    FullName = userToInvite.FullName,
                    Email = userToInvite.Email,
                    CreatedAt = userToInvite.CreatedAt
                },
                Role = ProjectRole.Member,
                JoinedAt = membership.JoinedAt
            };
        }

        public async Task<IEnumerable<ProjectMemberDto>> GetProjectMembersAsync(int projectId, int userId)
        {
            var isMember = await IsUserProjectMemberAsync(projectId, userId);
            if (!isMember)
            {
                throw new UnauthorizedAccessException("Access denied");
            }

            var members = await _context.ProjectMembers
                .Where(pm => pm.ProjectId == projectId)
                .Include(pm => pm.User)
                .Select(pm => new ProjectMemberDto
                {
                    User = new UserDto
                    {
                        Id = pm.User.Id,
                        FullName = pm.User.FullName,
                        Email = pm.User.Email,
                        CreatedAt = pm.User.CreatedAt
                    },
                    Role = pm.Role,
                    JoinedAt = pm.JoinedAt
                })
                .ToListAsync();

            return members;
        }

        public async Task<bool> IsUserProjectOwnerAsync(int projectId, int userId)
        {
            return await _context.Projects
                .AnyAsync(p => p.Id == projectId && p.OwnerId == userId);
        }

        public async Task<bool> IsUserProjectMemberAsync(int projectId, int userId)
        {
            return await _context.Projects
                .AnyAsync(p => p.Id == projectId &&
                              (p.OwnerId == userId || p.Members.Any(m => m.UserId == userId)));
        }
    }
}
