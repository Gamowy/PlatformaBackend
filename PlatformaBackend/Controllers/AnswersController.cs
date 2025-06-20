using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platforma.Application.Answers;
using Platforma.Application.Assignments;
using Platforma.Domain;

namespace PlatformaBackend.Controllers
{
    public class AnswersController : BaseAPIController
    {
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
            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Get all answers of a user in a given course
        /// </summary>
        [HttpGet("course/{courseId}/user")]
        [Authorize]
        public async Task<ActionResult<List<Answer>>> GetAllAnswersForUserInCourse(Guid courseId)
        {
            var finalUserId = Guid.Parse(HttpContextAccessor.HttpContext!.User.FindFirst("UserId")!.Value);

            var result = await Mediator.Send(new GetAllAnswerForUserInCourse.Query
            {
                CourseId = courseId,
                UserId = finalUserId
            });

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        /// <summary>
        /// Get details about specified answer
        /// </summary>
        [HttpGet("details/{answerId}")]
        public async Task<ActionResult<Answer>> GetAnswerDetails(Guid answerId)
        {
            var result = await Mediator.Send(new GetAnswerDetails.Query { AnswerId = answerId });
            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
            {
                if (await OwnerOfAnswerOrTeacher(result.Value))
                    return Ok(result.Value);
                else
                    return Forbid();
            }
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Get number of marked and unmarked answers for each assignment in a course
        /// </summary>
        [HttpGet("course/{courseId}/count")]
        [Authorize(Roles = Platforma.Domain.User.Roles.Teacher)]
        public async Task<ActionResult<NumberOfAnswersDTO>> GetNumberOfAnswers(Guid courseId)
        {
            if (!await UserParticipateInCourse(courseId))
                return Forbid();

            var result = await Mediator.Send(new GetNumberOfAnswers.Query { CourseId = courseId });
            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Get number of marked and unmarked answers for each assigment of courses for specified teacher
        /// </summary>
        [HttpGet("course/userCount")]
        [Authorize(Roles = Platforma.Domain.User.Roles.Teacher)]
        public async Task<ActionResult<NumberOfAnswersWithNameDTO>> GetNumberOfAnswersForSpecifiedUser()
        {
            var finalUserId = Guid.Parse(HttpContextAccessor.HttpContext!.User.FindFirst("UserId")!.Value);
            var result = await Mediator.Send(new GetNotificationListOfUnfinishedAssigment.Query { UserId = finalUserId });
            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }


        /// <summary>
        /// Get list of student with answear and mark
        /// </summary>
        [HttpGet("course/{courseId}/assigment/{assigmentId}")]
        [Authorize(Roles = Platforma.Domain.User.Roles.Teacher)]
        public async Task<ActionResult<NumberOfAnswersWithNameDTO>> GetListOfStudentWithAnswerAndMark(Guid courseId, Guid assigmentId)
        {
            if (!await UserParticipateInCourse(courseId))
                return Forbid();
            var result = await Mediator.Send(new GetListOfStudentWithAnswear.Query { CourseId = courseId, AssigmentId = assigmentId });
            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
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
            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }
    }
}