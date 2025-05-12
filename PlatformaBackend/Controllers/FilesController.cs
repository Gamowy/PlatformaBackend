using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> UploadAssignmentFile(Guid assignmentId, IFormFile file)
        {
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
        public async Task<IActionResult> RemoveAssignmentFile(Guid assignmentId)
        {
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
        public async Task<IActionResult> UploadAnswer(Guid assignmentId, IFormFile file)
        {
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
        public async Task<IActionResult> DeleteAnswer(Guid answerId)
        {
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
        public async Task<IActionResult> DownloadCourse(Guid courseId)
        {
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
