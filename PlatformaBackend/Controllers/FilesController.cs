using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platforma.Application;
using Platforma.Application.Answers;
using Platforma.Application.Assignments;
using Platforma.Application.Files;
using Platforma.Domain;
using System.Security.Claims;

namespace PlatformaBackend.Controllers
{
    public class FilesController : BaseAPIController
    {
        private readonly IHttpContextAccessor _HttpContextAccessor;
        public FilesController(IHttpContextAccessor httpContextAccessor)
        {
            _HttpContextAccessor = httpContextAccessor;
        }

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

            var assignmentDetails = await Mediator.Send(new AssignmentDetails.Query { AssignmentId = answer.Value.AssignmentId });

            if (_HttpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Role)!.Value.Equals(Platforma.Domain.User.Roles.Student) &&
                !answer.Value.UserId.Equals(Guid.Parse(_HttpContextAccessor.HttpContext.User.FindFirst("UserId")!.Value)))
                return Forbid();
            else if (!assignmentDetails.IsSuccess || assignmentDetails.Value == null)
                return BadRequest("Couldn't get authorization details");
            else if (!await UserParticipateInCourse(assignmentDetails.Value.CourseId))
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


            var userId = Guid.Parse(_HttpContextAccessor.HttpContext!.User.FindFirst("UserId")!.Value);
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
            else if (!answerDetails.Value.UserId.Equals(Guid.Parse(_HttpContextAccessor.HttpContext!.User.FindFirst("UserId")!.Value)))
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
