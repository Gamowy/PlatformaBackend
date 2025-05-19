using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platforma.Application.Answers;
using Platforma.Application.Assignments;
using Platforma.Application.Files;
using Platforma.Domain;
using System.Security.Claims;


namespace PlatformaBackend.Controllers
{
    public class AssignmentsController : BaseAPIController
    {
        /// <summary>
        /// Get a list of all assignments for specified course
        /// </summary>
        [HttpGet("{courseId}")]
        public async Task<ActionResult<List<Assignment>>> GetAllAssignments(Guid courseId)
        {
            if (!await UserParticipateInCourse(courseId))
                return Forbid();

            var result = await Mediator.Send(new GetAssignments.Query { CourseId = courseId });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
            {
                var AssignmentList = result.Value;
                if (HttpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Role)!.Value.Equals(Platforma.Domain.User.Roles.Student))
                    AssignmentList = result.Value.Where(a => a.OpenDate != null).ToList();
                return Ok(AssignmentList);
            }
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
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
            {
                if (!await UserParticipateInCourse(result.Value.CourseId) ||
                    (HttpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Role)!.Value.Equals(Platforma.Domain.User.Roles.Student) && result.Value.OpenDate == null))
                    return Forbid();
                return Ok(result.Value);
            }
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Create a new assignment for course
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Platforma.Domain.User.Roles.Teacher)]
        public async Task<IActionResult> CreateNewAssignment(AssignmentDTORequest assignmentDTO)
        {
            if (!await UserParticipateInCourse(assignmentDTO.CourseId))
                return Forbid();

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
        [Authorize(Policy = "AdminOrTeacher")]
        public async Task<IActionResult> EditAssignment(Guid assignmentId, AssignmentDTORequest assignmentDTO)
        {
            if (!await UserParticipateInCourse(assignmentDTO.CourseId))
                return Forbid();

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
        [Authorize(Policy = "AdminOrTeacher")]
        public async Task<IActionResult> DeleteAssignment(Guid assignmentId)
        {
            var resultDetails = await Mediator.Send(new AssignmentDetails.Query { AssignmentId = assignmentId });

            if (resultDetails.IsSuccess && resultDetails.Value != null)
            {
                if (!await UserParticipateInCourse(resultDetails.Value.CourseId))
                    return Forbid();

                // Usuwanie pliku jeżeli dołączony
                if (!resultDetails.Value.FilePath.Equals("") && resultDetails.Value.FilePath != null)
                {
                    var fileDeleteResult = await Mediator.Send(new RemoveAssignmentFile.Command { AssignmentId = assignmentId });
                    if (!fileDeleteResult.IsSuccess || fileDeleteResult.Value == null)
                        return BadRequest("Couldn't remove bounded file");
                }

                // Usuwanie odpowiedzi do usuwanego zadania
                if (resultDetails.Value!.AnswerRequired)
                {
                    var answersResult = await Mediator.Send(new GetAllAssignmentAnswers.Query { AssignmentId = assignmentId });
                    if (answersResult.IsSuccess && answersResult.Value != null)
                        foreach (var ar in answersResult.Value)
                        {
                            var deleteAnswer = await Mediator.Send(new DeleteAnswerFile.Command { AnswerId = ar.Id });
                            if (!deleteAnswer.IsSuccess || deleteAnswer.Value == null)
                                return BadRequest("Couldn't remove all bounded answers");
                        }
                }
            }
            else
                return BadRequest("Couldn't remove assignment data");

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
