using Microsoft.AspNetCore.Mvc;
using Platforma.Application.Files;

namespace PlatformaBackend.Controllers
{
    public class FilesController : BaseAPIController
    {
        [HttpGet("file/{assignmentId}")]
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

        [HttpPut("file/{assignmentId}")]
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

        [HttpDelete("file/{assignmentId}")]
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

        [HttpGet("answers/{id}")]
        public async Task<IActionResult> DownloadAnswer(Guid id)
        {
            var result = await Mediator.Send(new DownloadAnswerFile.Query { AnswerId = id });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return result.Value;
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        [HttpPut("answers/{id}")]
        public async Task<IActionResult> UploadAnswer(Guid id, IFormFile file)
        {
            var result = await Mediator.Send(new UploadAnswerFile.Command { AnswerId = id, File = file });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        [HttpDelete("answers/{id}")]
        public async Task<IActionResult> DeleteAnswer(Guid id)
        {
            var result = await Mediator.Send(new DeleteAnswerFile.Command { AnswerId = id });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        [HttpGet("course/{id}")]
        public async Task<IActionResult> DownloadCourse(Guid id)
        {
            var result = await Mediator.Send(new DownloadAllCourseFiles.Query { CourseId = id });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return result.Value;
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }
    }
}
