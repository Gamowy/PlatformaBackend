using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platforma.Application.Accounts;
using Platforma.Application.Accounts.DTOs;
using Platforma.Application.Users;

namespace PlatformaBackend.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseAPIController
    {
        /// <summary>
        /// Used to login
        /// </summary>
        [HttpPost("login")]
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
        public async Task<IActionResult> RegisterUser(UserRegisterDTO userRegisterDTO)
        {
            var result = await Mediator.Send(new Register.Command { UserRegisterDTO = userRegisterDTO });

            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }

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
    }
}
