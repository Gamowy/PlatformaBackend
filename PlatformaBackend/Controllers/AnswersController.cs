using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platforma.Application.Answers;
using Platforma.Application.Assignments;
using Platforma.Domain;
using System.Security.Claims;

namespace PlatformaBackend.Controllers
{
    public class AnswersController : BaseAPIController
    {
        private readonly IHttpContextAccessor _HttpContextAccessor;
        public AnswersController(IHttpContextAccessor httpContextAccessor)
        {
            _HttpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get a list of all answers for specified assignment
        /// </summary>
        [HttpGet("{assignmentId}")]
        [Authorize(Policy = "AdminOrTeacher")]
        public async Task<ActionResult<List<Answer>>> GetAllAnswers(Guid assignmentId)
        {
            var assignmentDetails = await Mediator.Send(new AssignmentDetails.Query { AssignmentId = assignmentId });
            if(!assignmentDetails.IsSuccess || assignmentDetails.Value == null)
                return BadRequest("Couldn't get assignment details");

            if (!await UserParticipateInCourse(assignmentDetails.Value.CourseId))
                return Forbid();

            var result = await Mediator.Send(new GetAllAssignmentAnswers.Query { AssignmentId = assignmentId });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Get details about specified answer
        /// </summary>
        [HttpGet("details/{answerId}")]
        public async Task<ActionResult<Answer>> GetAnswerDetails(Guid answerId)
        {
            var result = await Mediator.Send(new GetAnswerDetails.Query { AnswerId = answerId });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
            {
                var assignmentDetails = await Mediator.Send(new AssignmentDetails.Query { AssignmentId = result.Value.AssignmentId });

                if (_HttpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Role)!.Value.Equals(Platforma.Domain.User.Roles.Student) &&
                    !result.Value.UserId.Equals(Guid.Parse(_HttpContextAccessor.HttpContext.User.FindFirst("UserId")!.Value)))
                    return Forbid();
                else if (!assignmentDetails.IsSuccess || assignmentDetails.Value == null)
                    return BadRequest("Couldn't get authorization details");
                else if (!await UserParticipateInCourse(assignmentDetails.Value.CourseId))
                    return Forbid();
                else
                    return Ok(result.Value);
            }
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Used to mark answers as teacher
        /// </summary>
        [HttpPut("mark/{answerId}")]
        [Authorize(Roles = Platforma.Domain.User.Roles.Teacher)]
        public async Task<IActionResult> MarkAnswer(Guid answerId, MarkAnswerDTO markAnswerDTO)
        {
            var answerDetails = await Mediator.Send(new GetAnswerDetails.Query { AnswerId = answerId });
            if (!answerDetails.IsSuccess || answerDetails.Value == null)
                return BadRequest("Couldn't get authorization details");

            var assignmentDetails = await Mediator.Send(new AssignmentDetails.Query { AssignmentId = answerDetails.Value.AssignmentId });

            if (!assignmentDetails.IsSuccess || assignmentDetails.Value == null)
                return BadRequest("Couldn't get authorization details");
            else if (!await UserParticipateInCourse(assignmentDetails.Value.CourseId))
                return Forbid();

            var result = await Mediator.Send(new MarkAnswer.Command { AnswerId = answerId, MarkAnswerDTO = markAnswerDTO });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        private async Task<bool> UserParticipateInCourse(Guid courseId)
        {
            //Admin może wszystko
            if (_HttpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Role)!.Value.Equals(Platforma.Domain.User.Roles.Administrator))
                return true;

            var course = await Mediator.Send(new Platforma.Application.Courses.Details.Query { id = courseId });
            //owner
            if (course.IsSuccess && course.Value.OwnerId.Equals(Guid.Parse(_HttpContextAccessor.HttpContext.User.FindFirst("UserId")!.Value)))
                return true;

            //lub uczestnik
            var courseUsers = await Mediator.Send(new Platforma.Application.Courses.UserList.Query { CourseId = courseId });
            if (courseUsers.IsSuccess &&
                courseUsers.Value.Where(u => u.Id.Equals(Guid.Parse(_HttpContextAccessor.HttpContext.User.FindFirst("UserId")!.Value)) &&
                u.status == UserStatus.Accepted).FirstOrDefault() != null)
                return true;

            return false;
        }
    }
}