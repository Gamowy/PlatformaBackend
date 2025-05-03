using Microsoft.AspNetCore.Mvc;
using Platforma.Application.Files;

namespace PlatformaBackend.Controllers
{
    public class FilesController : BaseAPIController
    {
        [HttpPost("assignments")]
        public async Task<IActionResult> UploadAssignment(AssignmentUploadDTO assignmentUploadDTO)
        {
            var result = await Mediator.Send(new UploadAssignment.Command { AssignmentUploadDTO = assignmentUploadDTO });
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
