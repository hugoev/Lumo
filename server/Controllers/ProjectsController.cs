using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Lumo.DTOs;
using Lumo.Services;

namespace Lumo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }



        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User ID not found in token."));
                var projects = await _projectService.GetProjectsForUserAsync(userId).ConfigureAwait(false);
                return Ok(projects);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "You are not authorized to perform this action." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User ID not found in token."));
                var project = await _projectService.GetProjectByIdAsync(id, userId).ConfigureAwait(false);
                return Ok(project);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Project not found." });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "You are not authorized to perform this action." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto createDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User ID not found in token."));
                var project = await _projectService.CreateProjectAsync(createDto, userId).ConfigureAwait(false);
                return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "You are not authorized to perform this action." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectDto updateDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User ID not found in token."));
                var project = await _projectService.UpdateProjectAsync(id, updateDto, userId).ConfigureAwait(false);
                return Ok(project);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Project not found." });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "You are not authorized to perform this action." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User ID not found in token."));
                await _projectService.DeleteProjectAsync(id, userId).ConfigureAwait(false);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Project not found." });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "You are not authorized to perform this action." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("{id}/invite")]
        public async Task<IActionResult> InviteMember(int id, [FromBody] InviteMemberDto inviteDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User ID not found in token."));
                var member = await _projectService.InviteMemberAsync(id, inviteDto, userId).ConfigureAwait(false);
                return Ok(member);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Project not found." });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "You are not authorized to perform this action." });
            }
            catch (InvalidOperationException)
            {
                return BadRequest(new { message = "An error occurred while processing your request." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("{id}/members")]
        public async Task<IActionResult> GetProjectMembers(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User ID not found in token."));
                var members = await _projectService.GetProjectMembersAsync(id, userId).ConfigureAwait(false);
                return Ok(members);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Project not found." });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "You are not authorized to perform this action." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
