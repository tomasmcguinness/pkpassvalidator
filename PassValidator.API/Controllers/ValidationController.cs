using Microsoft.AspNetCore.Mvc;
using PassValidator.API.Models;
using PassValidator.Lib.Validation;

namespace PassValidator.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValidationController : ControllerBase
    {
        [HttpPost]
        public IActionResult Validate([FromBody] PkPass pkPass)
        {
            return Ok(new Validator().Validate(
                System.Convert.FromBase64String(pkPass.EncodedBytes)));
        }
    }
}
