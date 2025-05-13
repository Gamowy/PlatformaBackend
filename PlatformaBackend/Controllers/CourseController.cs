using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Platforma.Application;
using Platforma.Application.Courses;
using Platforma.Application.Courses.DTOs;
using Platforma.Domain;
using System.Security.Claims;

namespace PlatformaBackend.Controllers
{
    public class CourseController : BaseAPIController
    {
        private readonly IHttpContextAccessor _HttpContextAccessor;

        public CourseController(IHttpContextAccessor httpContextAccessor) {
            _HttpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get a list of all courses
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Course>>> GetCourses()
        {
            var result = await Mediator.Send(new CourseList.Query());
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Get a list of users for specifed course
        /// </summary>
        [HttpGet("{courseId}/users")]
        public async Task<ActionResult<List<UserCourseDTO>>> GetUsersForCourse(Guid courseId)
        {
            var result = await Mediator.Send(new UserList.Query { CourseId = courseId });

            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();

            return BadRequest(result.Error);
        }

        /// <summary>
        /// Get details about specified course
        /// </summary>
        [HttpGet("{courseId}")] 
        public async Task<ActionResult<Course>> GetCourse(Guid courseId)
        {
            var result = await Mediator.Send(new Details.Query { id = courseId });

            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);

        }

        /// <summary>
        /// Edit course
        /// </summary>
        [HttpPut("{courseId}")]
        [Authorize(Policy = "AdminOrTeacher")]
        public async Task<IActionResult> EditCourse(Guid courseId, CourseEditDTO course)
        {
            if (!await CheckIfAdminOrOwner(courseId))
                return Forbid();

            var result = await Mediator.Send(new Edit.Command { CourseDTO = course, CourseId = courseId });

            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Create a new course
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "AdminOrTeacher")]
        public async Task<IActionResult> CreateCourse(CourseCreateDTO course)
        {
            var result = await Mediator.Send(new Create.Command { 
                CourseDTO = course,
                UserId = Guid.Parse(_HttpContextAccessor.HttpContext!.User.FindFirst("UserId")!.Value)
            });

            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Delete a course
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrTeacher")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            if (!await CheckIfAdminOrOwner(id))
                return Forbid();

            var result = await Mediator.Send(new Delete.Command { Id = id });

            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        private async Task<bool> CheckIfAdminOrOwner(Guid courseId)
        {
            if (_HttpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Role)!.Value.Equals(Platforma.Domain.User.Roles.Administrator))
                return true;

            var courseToCheck = await Mediator.Send(new Details.Query { id = courseId });
            if (courseToCheck.IsSuccess && courseToCheck.Value.OwnerId.Equals(Guid.Parse(_HttpContextAccessor.HttpContext!.User.FindFirst("UserId")!.Value)))
                return true;

            return false;
        }
    }

}
