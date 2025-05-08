using Microsoft.AspNetCore.Mvc;
using Platforma.Application.Answers;
using Platforma.Application.Answers.DTO;

namespace PlatformaBackend.Controllers
{
    public class AnswersController : BaseAPIController
    {
        [HttpGet("{assignmentId}")]
        public async Task<ActionResult<List<AnswerDTOResponse>>> GetAllAnswers(Guid assignmentId)
        {
            var result = await Mediator.Send(new GetAllAssignmentAnswers.Query { AssignmentId = assignmentId });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        [HttpGet("details/{answerId}")]
        public async Task<ActionResult<AnswerDTOResponse>> GetAnswerDetails(Guid answerId)
        {
            var result = await Mediator.Send(new GetAnswerDetails.Query { AnswerId = answerId });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        [HttpPut("mark/{answerId}")]
        public async Task<IActionResult> MarkAnswer(Guid answerId, MarkAnswerDTO markAnswerDTO)
        {
            var result = await Mediator.Send(new MarkAnswer.Command { AnswerId = answerId, MarkAnswerDTO = markAnswerDTO });
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