using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platforma.Application;
using Platforma.Application.Answers;
using Platforma.Application.Assignments;
using Platforma.Application.Files;
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
        public async Task<ActionResult<List<Assignment>>> GetAllAssignments(Guid courseId)
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
        public async Task<ActionResult<Assignment>> GetAssignmentDetails(Guid assignmentId)
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
        [HttpPut("{assignmentId}")]
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
        [HttpDelete("{assignmentId}")]
        public async Task<IActionResult> DeleteAssignment(Guid assignmentId)
        {
            var resultDetails = await Mediator.Send(new AssignmentDetails.Query { AssignmentId = assignmentId });

            // Usuwanie pliku jeżeli dołączony
            if (resultDetails.IsSuccess && resultDetails.Value != null &&
                !resultDetails.Value.FilePath.Equals("") && resultDetails.Value.FilePath != null)
            {
                var fileDeleteResult = await Mediator.Send(new RemoveAssignmentFile.Command { AssignmentId = assignmentId });
                if (!fileDeleteResult.IsSuccess || fileDeleteResult.Value == null)
                    return BadRequest("Couldn't remove bounded file");
            }

            // Usuwanie odpowiedzi do usuwanego zadania
            if (resultDetails.IsSuccess && resultDetails.Value != null && resultDetails.Value.AnswerRequired)
            {
                var answersResult = await Mediator.Send(new GetAllAssignmentAnswers.Query { AssignmentId = assignmentId });
                if (answersResult.IsSuccess && answersResult.Value != null)
                    foreach(var ar in answersResult.Value)
                    {
                        var deleteAnswer = await Mediator.Send(new DeleteAnswerFile.Command { AnswerId = ar.Id });
                        if (!deleteAnswer.IsSuccess || deleteAnswer.Value == null)
                            return BadRequest("Couldn't remove all bounded answers");
                    }
            }

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
