using Microsoft.AspNetCore.Mvc;
using Platforma.Domain;
using Platforma.Application.Users;
using Microsoft.AspNetCore.Authorization;
using Platforma.Application.Courses;

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
    }
}
