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
        [HttpPost("login")]
        public async Task<ActionResult<String?>> GetToken(UserLoginDTO userLoginDTO)
        {
            var result = await Mediator.Send(new Login.Query { UserLoginDTO = userLoginDTO });
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(UserRegisterDTO userRegisterDTO)
        {
            var result = await Mediator.Send(new Register.Command { UserRegisterDTO = userRegisterDTO });

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
