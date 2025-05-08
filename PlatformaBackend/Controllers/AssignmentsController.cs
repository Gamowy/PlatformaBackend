using Microsoft.AspNetCore.Mvc;
using Platforma.Application;
using Platforma.Application.Assignments;


namespace PlatformaBackend.Controllers
{
    public class AssignmentsController : BaseAPIController
    {
        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetAllAssignments(Guid courseId)
        {
            var result = await Mediator.Send(new GetAssignments.Query { CourseId = courseId });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        [HttpGet("{courseId}/{assignmentId}")]
        public async Task<IActionResult> GetAssignmentDetails(Guid courseId, Guid assignmentId)
        {
            var result = await Mediator.Send(new AssignmentDetails.Query { CourseId = courseId, AssignmentId = assignmentId });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        [HttpPost("{courseId}")]
        public async Task<IActionResult> CreateNewAssignment(Guid courseId, AssignmentDTO assignmentDTO)
        {
            var result = await Mediator.Send(new CreateAssignment.Command { CourseId = courseId, AssignmentDTO = assignmentDTO });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        [HttpPut("{courseId}/{assignmentId}")]
        public async Task<IActionResult> EditAssignment(Guid courseId, Guid assignmentId, AssignmentDTO assignmentDTO)
        {
            var result = await Mediator.Send(new EditAssignment.Command { CourseId = courseId, AssignmentId = assignmentId, AssignmentDTO = assignmentDTO });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        [HttpDelete("{courseId}/{assignmentId}")]
        public async Task<IActionResult> DeleteAssignment(Guid courseId, Guid assignmentId)
        {
            var result = await Mediator.Send(new DeleteAssignment.Command {CourseId = courseId, AssignmentId = assignmentId });
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
