using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platforma.Application;
using Platforma.Application.CourseUsers;
using Platforma.Domain;
using System.Security.Claims;

namespace PlatformaBackend.Controllers
{
    public class CourseUserController : BaseAPIController
    {
        private readonly IHttpContextAccessor _HttpContextAccessor;
        public CourseUserController(IHttpContextAccessor httpContextAccessor) { 
            _HttpContextAccessor = httpContextAccessor;
        }

        [Authorize(Policy = "AdminOrTeacher")]
        [HttpPost("Teacher")]
        public async Task<IActionResult> AssignStudentToCourse(Guid courseId, Guid userId)
        {
            if (!await AuthorizeTeacherOrAdminInCourse(courseId))
                return Forbid();

            Result<Unit> result = await Mediator.Send(new AssignRequest.Command() {CourseId = courseId, UserId = userId, status = UserStatus.Accepted});

            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }

        [Authorize(Policy = "NotAdmin")]
        [HttpPost]
        public async Task<IActionResult> AssignYourselfToCourse(Guid courseId)
        {
            Result<Unit> result = await Mediator.Send(new AssignRequest.Command()
            {
                CourseId = courseId,
                UserId = Guid.Parse(_HttpContextAccessor.HttpContext.User.FindFirst("UserId").Value)
            });

            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }


        [Authorize(Policy = "AdminOrTeacher")]
        [HttpPut("Teacher")]
        public async Task<IActionResult> AcceptUserToCourse(Guid courseId, Guid userId)
        {
            if (! await AuthorizeTeacherOrAdminInCourse(courseId))
                return Forbid();

            Result<Unit> result = await Mediator.Send(new AcceptUser.Command() { CourseId = courseId, UserId = userId });

            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }

        [Authorize(Policy = "AdminOrTeacher")]
        [HttpDelete("Teacher")]
        public async Task<IActionResult> RemoveUserFromCourse(Guid courseId, Guid userId)
        {
            if (!await AuthorizeTeacherOrAdminInCourse(courseId))
                return Forbid();
            if (userId.Equals(Guid.Parse(_HttpContextAccessor.HttpContext.User.FindFirst("UserId").Value)))
                return BadRequest("You can't remove yourself");

            Result<Unit> result = await Mediator.Send(new RemoveUser.Command() { CourseId = courseId, UserId = userId });

            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }

        private async Task<bool> AuthorizeTeacherOrAdminInCourse(Guid courseId)
        {
            //Admin może wszystko
            if (_HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value.Equals(Platforma.Domain.User.Roles.Administrator))
                return true;

            var course = await Mediator.Send(new Platforma.Application.Courses.Details.Query { id = courseId });
            //owner lub współprowadzący może przypisać kogoś do kursu
            if (course.IsSuccess && course.Value.OwnerId.Equals(Guid.Parse(_HttpContextAccessor.HttpContext.User.FindFirst("UserId").Value)))
                return true;

            var courseUsers = await Mediator.Send(new Platforma.Application.Courses.UserList.Query { CourseId = courseId });
            if (courseUsers.IsSuccess &&
                courseUsers.Value.Where(u => u.Id.Equals(Guid.Parse(_HttpContextAccessor.HttpContext.User.FindFirst("UserId").Value)) &&
                u.status == UserStatus.Accepted).FirstOrDefault() != null)
                return true;

            return false;
        }
    }
}
