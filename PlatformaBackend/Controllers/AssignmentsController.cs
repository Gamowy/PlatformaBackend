using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platforma.Application;
using Platforma.Application.Assignments;
using Platforma.Application.Assignments.DTO;
using Platforma.Domain;
using System.Runtime.InteropServices;
using System.Security.Claims;


namespace PlatformaBackend.Controllers
{
    public class AssignmentsController : BaseAPIController
    {
        private readonly IHttpContextAccessor _HttpContextAccessor;
        public AssignmentsController(IHttpContextAccessor httpContextAccessor)
        {
            _HttpContextAccessor = httpContextAccessor;
        }


        [HttpGet("{courseId}")]
        public async Task<ActionResult<List<AssignmentDTOResponse>>> GetAllAssignments(Guid courseId)
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

        [HttpGet("details/{assignmentId}")]
        public async Task<ActionResult<AssignmentDTOResponse>> GetAssignmentDetails(Guid assignmentId)
        {
            var result = await Mediator.Send(new AssignmentDetails.Query { AssignmentId = assignmentId });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        [Authorize(Policy = "AdminOrTeacher")]
        [HttpPost]
        public async Task<IActionResult> CreateNewAssignment(AssignmentDTORequest assignmentDTO)
        {
            var result = await Mediator.Send(new CreateAssignment.Command { AssignmentDTO = assignmentDTO });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        [HttpPut("details/{assignmentId}")]
        public async Task<IActionResult> EditAssignment(Guid assignmentId, AssignmentDTORequest assignmentDTO)
        {
            var result = await Mediator.Send(new EditAssignment.Command { AssignmentId = assignmentId, AssignmentDTO = assignmentDTO });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        [HttpDelete("details/{assignmentId}")]
        public async Task<IActionResult> DeleteAssignment(Guid assignmentId)
        {
            var result = await Mediator.Send(new DeleteAssignment.Command { AssignmentId = assignmentId });
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
