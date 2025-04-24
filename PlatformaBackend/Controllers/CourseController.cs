using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platforma.Application;
using Platforma.Application.Courses;
using Platforma.Domain;

namespace PlatformaBackend.Controllers
{
    [Authorize]
    public class CourseController : BaseAPIController
    {
        [HttpGet]
        public async Task<ActionResult<List<Course>>> GetCourses()
        {
            var result = await Mediator.Send(new List.Query());
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

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

        [HttpGet("{id}")] 
        public async Task<ActionResult<Course>> GetCar(Guid id)
        {
            var result = await Mediator.Send(new Details.Query { id = id });

            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditCar(Guid id, Course course)
        {
            course.Id = id;
            var result = await Mediator.Send(new Edit.Command { Course = course });

            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }
        [HttpPost]
        public async Task<IActionResult> CreateCar(Course course)
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(Guid id)
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
