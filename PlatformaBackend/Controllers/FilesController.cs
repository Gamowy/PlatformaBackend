using Microsoft.AspNetCore.Mvc;
using Platforma.Application.Files;

namespace PlatformaBackend.Controllers
{
    public class FilesController : BaseAPIController
    {
        [HttpPost("assignments")]
        public async Task<IActionResult> UploadAssignment(AssignmentUploadDTO assignmentUploadDTO)
        {
            var result = await Mediator.Send(new UploadAssignmentFile.Command { AssignmentUploadDTO = assignmentUploadDTO });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        [HttpDelete("assignments/{id}")]
        public async Task<IActionResult> DeleteAssignment(Guid id)
        {
            var result = await Mediator.Send(new DeleteAssignmentFile.Command { AssignmentId = id });
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
