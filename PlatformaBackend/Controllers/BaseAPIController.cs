using Microsoft.AspNetCore.Mvc;
using MediatR;
using Platforma.Domain;
using System.Security.Claims;
using Platforma.Application.Assignments;


namespace PlatformaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseAPIController : ControllerBase
    {
        private IMediator? _mediator;

        protected IMediator Mediator => _mediator ??=
            HttpContext.RequestServices.GetService<IMediator>()!;

        private IHttpContextAccessor? _httpContextAccessor;

        protected IHttpContextAccessor HttpContextAccessor => _httpContextAccessor ??=
            HttpContext.RequestServices.GetService<IHttpContextAccessor>()!;

        protected async Task<bool> OwnerOfAnswerOrTeacher(Answer answer)
        {
            var assignmentDetails = await Mediator.Send(new AssignmentDetails.Query { AssignmentId = answer.AssignmentId });

            if (HttpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Role)!.Value.Equals(Platforma.Domain.User.Roles.Student) &&
                !answer.UserId.Equals(Guid.Parse(HttpContextAccessor.HttpContext.User.FindFirst("UserId")!.Value)))
                return false;
            else if (!assignmentDetails.IsSuccess || assignmentDetails.Value == null)
                return false;
            else if (!await UserParticipateInCourse(assignmentDetails.Value.CourseId))
                return false;

            return true;
        }

        protected async Task<bool> UserParticipateInCourse(Guid courseId)
        {
            if(await CheckIfAdminOrOwner(courseId))
                return true;

            var courseUsers = await Mediator.Send(new Platforma.Application.Courses.UserList.Query { CourseId = courseId,onlyAccepted = false });
            if (courseUsers.IsSuccess &&
                courseUsers.Value.Where(u => u.Id.Equals(Guid.Parse(HttpContextAccessor.HttpContext!.User.FindFirst("UserId")!.Value)) &&
                u.status == UserStatus.Accepted).FirstOrDefault() != null)
                return true;

            return false;
        }

        protected async Task<bool> CheckIfAdminOrOwner(Guid courseId)
        {
            //Admin może wszystko
            if (HttpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Role)!.Value.Equals(Platforma.Domain.User.Roles.Administrator))
                return true;

            var course = await Mediator.Send(new Platforma.Application.Courses.Details.Query { id = courseId });
            //owner
            if (course.IsSuccess && course.Value.OwnerId.Equals(Guid.Parse(HttpContextAccessor.HttpContext.User.FindFirst("UserId")!.Value)))
                return true;

            return false;
        }
    }
}

