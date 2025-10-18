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
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }



        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetTasksForProject(int projectId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User ID not found in token."));
                var tasks = await _taskService.GetTasksForProjectAsync(projectId, userId).ConfigureAwait(false);
                return Ok(tasks);
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
        public async Task<IActionResult> GetTask(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User ID not found in token."));
                var task = await _taskService.GetTaskByIdAsync(id, userId).ConfigureAwait(false);
                return Ok(task);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "The requested resource was not found." });
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

        [HttpPost("project/{projectId}")]
        public async Task<IActionResult> CreateTask(int projectId, [FromBody] CreateTaskDto createDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User ID not found in token."));
                var task = await _taskService.CreateTaskAsync(createDto, projectId, userId).ConfigureAwait(false);
                return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
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
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User ID not found in token."));
                var task = await _taskService.UpdateTaskAsync(id, updateDto, userId).ConfigureAwait(false);
                return Ok(task);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "The requested resource was not found." });
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
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User ID not found in token."));
                await _taskService.DeleteTaskAsync(id, userId).ConfigureAwait(false);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "The requested resource was not found." });
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

        [HttpPut("update-statuses")]
        public async Task<IActionResult> UpdateTaskStatuses([FromBody] IEnumerable<UpdateTaskStatusDto> updates)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User ID not found in token."));
                var updatedTasks = await _taskService.UpdateTaskStatusesAsync(updates, userId).ConfigureAwait(false);
                return Ok(updatedTasks);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "The requested resource was not found." });
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
