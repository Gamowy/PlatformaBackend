using Microsoft.AspNetCore.Mvc;
using Platforma.Application.Answers;
using Platforma.Domain;

namespace PlatformaBackend.Controllers
{
    public class AnswersController : BaseAPIController
    {
        /// <summary>
        /// Get a list of all answers for specified assignment
        /// </summary>
        [HttpGet("{assignmentId}")]
        public async Task<ActionResult<List<Answer>>> GetAllAnswers(Guid assignmentId)
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

        /// <summary>
        /// Get details about specified answer
        /// </summary>
        [HttpGet("details/{answerId}")]
        public async Task<ActionResult<Answer>> GetAnswerDetails(Guid answerId)
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

        /// <summary>
        /// Used to mark answers as teacher
        /// </summary>
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