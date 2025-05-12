using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platforma.Application;
using Platforma.Application.Courses;
using Platforma.Domain;

namespace PlatformaBackend.Controllers
{
    public class CourseController : BaseAPIController
    {

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
        public async Task<ActionResult<List<User>>> GetUsersForCourse(Guid courseId)
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
        public async Task<IActionResult> EditCourse(Guid courseId, Course course)
        {
            course.Id = courseId;
            var result = await Mediator.Send(new Edit.Command { Course = course });

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
        public async Task<IActionResult> CreateCourse(Course course)
        {
            var result = await Mediator.Send(new Create.Command { Course = course });

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
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            var result = await Mediator.Send(new Delete.Command { Id = id });

            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }
    }

}
