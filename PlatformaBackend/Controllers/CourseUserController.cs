using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platforma.Application;
using Platforma.Application.CourseUsers;
using Platforma.Domain;

namespace PlatformaBackend.Controllers
{
    public class CourseUserController : BaseAPIController
    {
        /// <summary>
        /// Assign student to course
        /// </summary>
        [Authorize(Policy = "AdminOrTeacher")]
        [HttpPost("Teacher")]
        public async Task<IActionResult> AssignStudentToCourse(Guid courseId, Guid userId)
        {
            if (!await UserParticipateInCourse(courseId))
                return Forbid();

            Result<Unit?> result = await Mediator.Send(new AssignRequest.Command() {CourseId = courseId, UserId = userId, status = UserStatus.Accepted});

            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Assign yourself to course
        /// </summary>
        [Authorize(Policy = "NotAdmin")]
        [HttpPost]
        public async Task<IActionResult> AssignYourselfToCourse(Guid courseId)
        {
            Result<Unit?> result = await Mediator.Send(new AssignRequest.Command()
            {
                CourseId = courseId,
                UserId = Guid.Parse(HttpContextAccessor.HttpContext!.User.FindFirst("UserId")!.Value)
            });

            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }


        /// <summary>
        /// Accept user to course
        /// </summary>
        [Authorize(Policy = "AdminOrTeacher")]
        [HttpPut("Teacher")]
        public async Task<IActionResult> AcceptUserToCourse(Guid courseId, Guid userId)
        {
            if (! await UserParticipateInCourse(courseId))
                return Forbid();

            Result<Unit?> result = await Mediator.Send(new AcceptUser.Command() { CourseId = courseId, UserId = userId });

            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Remove user from course
        /// </summary>
        [Authorize(Policy = "AdminOrTeacher")]
        [HttpDelete("Teacher")]
        public async Task<IActionResult> RemoveUserFromCourse(Guid courseId, Guid userId)
        {
            if (!await UserParticipateInCourse(courseId))
                return Forbid();
            if (userId.Equals(Guid.Parse(HttpContextAccessor.HttpContext!.User.FindFirst("UserId")!.Value)))
                return BadRequest("You can't remove yourself");

            Result<Unit?> result = await Mediator.Send(new RemoveUser.Command() { CourseId = courseId, UserId = userId });

            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }
    }
}
