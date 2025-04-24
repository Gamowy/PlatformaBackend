using Microsoft.AspNetCore.Mvc;
using Platforma.Domain;
using Platforma.Application.Users;

namespace PlatformaBackend.Controllers
{
    public class LoginController : BaseAPIController
    {
        [HttpPost]
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
    }
}
