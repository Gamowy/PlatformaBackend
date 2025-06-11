using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platforma.Application.Accounts;
using Platforma.Application.Accounts.DTOs;
using Platforma.Application.Users;
using Platforma.Domain;
using System.Security.Claims;

namespace PlatformaBackend.Controllers
{
    public class AccountController : BaseAPIController
    {
        /// <summary>
        /// Used to login
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<String?>> GetToken(UserLoginDTO userLoginDTO)
        {
            var result = await Mediator.Send(new Login.Query { UserLoginDTO = userLoginDTO });
            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Used to register new user
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser(UserRegisterDTO userRegisterDTO)
        {
            var result = await Mediator.Send(new Register.Command { UserRegisterDTO = userRegisterDTO });

            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Used to change password for logged user
        /// </summary>
        [HttpPost("passwordChange")]
        public async Task<IActionResult> ChangePassword(PassChangeDTO passChangeDTO)
        {
            var result = await Mediator.Send(new EditPassword.Command
            {
                PassResetDTO = passChangeDTO,
                UserId = Guid.Parse(HttpContextAccessor.HttpContext!.User.FindFirst("UserId")!.Value)
            });

            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Used to get user role
        /// </summary>
        [HttpGet("role")]
        public IActionResult GetRole()
        {
            return Ok(HttpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Role)!.Value);
        }

        /// <summary>
        /// Used to apply for teacher role
        /// </summary>
        [HttpGet("teacherApply")]
        public async Task<IActionResult> TeacherApply()
        {

            var result = await Mediator.Send(new ApplyForTeacher.Command
            {
                UserId = Guid.Parse(HttpContextAccessor.HttpContext!.User.FindFirst("UserId")!.Value),
                Withdraw = false
            });

            if (result == null)
                return NotFound();
            if (result.IsSuccess)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Used to withdraw application for teacher role
        /// </summary>
        [HttpGet("teacherApplicationWithdraw")]
        public async Task<IActionResult> TeacherApplicationWithdraw()
        {
            var result = await Mediator.Send(new ApplyForTeacher.Command
            {
                UserId = Guid.Parse(HttpContextAccessor.HttpContext!.User.FindFirst("UserId")!.Value),
                Withdraw = true
            });

            if (result == null)
                return NotFound();
            if (result.IsSuccess)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Used to approve for teacher role
        /// </summary>
        [HttpGet("teacherApprove")]
        [Authorize(Roles = Platforma.Domain.User.Roles.Administrator)]
        public async Task<IActionResult> TeacherApprove(Guid UserId, bool Approve)
        {
            var result = await Mediator.Send(new TeacherApprove.Command
            {
                UserId = UserId,
                Approve = Approve
            });

            if (result == null)
                return NotFound();
            if (result.IsSuccess)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Used to show list of users applicants for teacher
        /// </summary>
        [HttpGet("teacherApplications")]
        [Authorize(Roles = Platforma.Domain.User.Roles.Administrator)]
        public async Task<IActionResult> GetTeacherApplications()
        {
            var result = await Mediator.Send(new TeacherApplicantsList.Query());

            if (result == null)
                return NotFound();
            if (result.IsSuccess)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Validate token
        /// </summary>
        [HttpGet("validate")]
        [AllowAnonymous]
        public IActionResult ValidateToken()
        {
            var context = HttpContextAccessor.HttpContext;
            if (context == null) return Ok(false);
            var expiration = context.User.FindFirstValue("exp");
            if (expiration == null) return Ok(false);

            return Ok(DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiration)) > DateTime.UtcNow);
        }
    }
}
