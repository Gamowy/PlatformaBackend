using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platforma.Application.Answers;
using Platforma.Application.Assignments;
using Platforma.Application.Files;

namespace PlatformaBackend.Controllers
{
    public class FilesController : BaseAPIController
    {

        #region Assignments
        /// <summary>
        /// Download assignment file
        /// </summary>
        [HttpGet("assignments/{assignmentId}")]
        public async Task<IActionResult> DownloadAssignmentFile(Guid assignmentId)
        {
            var assignmnet = await Mediator.Send(new AssignmentDetails.Query { AssignmentId = assignmentId });
            if (!assignmnet.IsSuccess || assignmnet.Value == null)
                return BadRequest("Couldn't get authorization details");
            else if (!await UserParticipateInCourse(assignmnet.Value.CourseId))
                return Forbid();

            var result = await Mediator.Send(new DownloadAssignmentFile.Query { AssignmentId = assignmentId });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return result.Value;
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Upload assignment file
        /// </summary>
        [HttpPut("assignments/{assignmentId}")]
        [Authorize(Policy = "AdminOrTeacher")]
        public async Task<IActionResult> UploadAssignmentFile(Guid assignmentId, IFormFile file)
        {
            var assignmnet = await Mediator.Send(new AssignmentDetails.Query { AssignmentId = assignmentId });
            if (!assignmnet.IsSuccess || assignmnet.Value == null)
                return BadRequest("Couldn't get authorization details");
            else if (!await UserParticipateInCourse(assignmnet.Value.CourseId))
                return Forbid();

            var result = await Mediator.Send(new UploadAssignmentFile.Command { AssignmentId = assignmentId, File = file });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Remove assignment file
        /// </summary>
        [HttpDelete("assignments/{assignmentId}")]
        [Authorize(Policy = "AdminOrTeacher")]
        public async Task<IActionResult> RemoveAssignmentFile(Guid assignmentId)
        {
            var assignmnet = await Mediator.Send(new AssignmentDetails.Query { AssignmentId = assignmentId });
            if (!assignmnet.IsSuccess || assignmnet.Value == null)
                return BadRequest("Couldn't get authorization details");
            else if (!await UserParticipateInCourse(assignmnet.Value.CourseId))
                return Forbid();

            var result = await Mediator.Send(new RemoveAssignmentFile.Command { AssignmentId = assignmentId });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }
        #endregion

        #region Answers
        /// <summary>
        /// Download specified answer file
        /// </summary>
        [HttpGet("answers/{answerId}")]
        public async Task<IActionResult> DownloadAnswer(Guid answerId)
        {
            var answer = await Mediator.Send(new GetAnswerDetails.Query { AnswerId = answerId });
            if (!answer.IsSuccess || answer.Value == null)
                return BadRequest("Couldn't get authorization details");

            if (!await OwnerOfAnswerOrTeacher(answer.Value))
                return Forbid();

            var result = await Mediator.Send(new DownloadAnswerFile.Query { AnswerId = answerId });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return result.Value;
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Upload new answer for assignment
        /// </summary>
        [HttpPost("answers")]
        [Authorize(Roles = Platforma.Domain.User.Roles.Student)]
        public async Task<IActionResult> UploadAnswer(Guid assignmentId, IFormFile file)
        {
            //tylko student może przesyłać odpowiedzi
            var assignmentDetails = await Mediator.Send(new AssignmentDetails.Query { AssignmentId = assignmentId });
            if (!assignmentDetails.IsSuccess || assignmentDetails.Value == null)
                return BadRequest("Couldn't get authorization details");
            else if (!await UserParticipateInCourse(assignmentDetails.Value.CourseId))
                return Forbid();


            var userId = Guid.Parse(HttpContextAccessor.HttpContext!.User.FindFirst("UserId")!.Value);
            var result = await Mediator.Send(new UploadAnswerFile.Command { UserId = userId, AssignmentId = assignmentId, File = file });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Delete answer to assignment
        /// </summary>
        [HttpDelete("answers/{answerId}")]
        [Authorize(Roles = Platforma.Domain.User.Roles.Student)]
        public async Task<IActionResult> DeleteAnswer(Guid answerId)
        {
            //tylko właściciel może usunąć przesłane zadanie
            var answerDetails = await Mediator.Send(new GetAnswerDetails.Query { AnswerId = answerId });
            if (!answerDetails.IsSuccess || answerDetails.Value == null)
                return BadRequest("Couldn't get authorization details");
            else if (!answerDetails.Value.UserId.Equals(Guid.Parse(HttpContextAccessor.HttpContext!.User.FindFirst("UserId")!.Value)))
                return Forbid();

            var result = await Mediator.Send(new DeleteAnswerFile.Command { AnswerId = answerId });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }
        #endregion

        #region Course
        /// <summary>
        /// Used to download all course files in .zip format
        /// </summary>
        [HttpGet("course/{courseId}")]
        [Authorize(Policy = "AdminOrTeacher")]
        public async Task<IActionResult> DownloadCourse(Guid courseId)
        {
            if (!await UserParticipateInCourse(courseId))
                return Forbid();

            var result = await Mediator.Send(new DownloadAllCourseFiles.Query { CourseId = courseId });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return result.Value;
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }
        #endregion

    }
}
