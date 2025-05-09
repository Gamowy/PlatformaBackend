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


        /// <summary>
        /// Get a list of all assignments for specified course
        /// </summary>
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

        /// <summary>
        /// Get details about specified assignment
        /// </summary>
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

        /// <summary>
        /// Create a new assignment for course
        /// </summary>
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

        /// <summary>
        /// Edit specified assignment
        /// </summary>
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

        /// <summary>
        /// Delete specified assignment
        /// </summary>
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
