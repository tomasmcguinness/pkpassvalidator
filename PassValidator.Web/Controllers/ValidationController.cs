using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PassValidator.Web.Validation;
using System.IO;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValidationController : ControllerBase
    {
        public IActionResult Post(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                ms.Position = 0;

                Validator validator = new Validator();
                var result = validator.Validate(ms.ToArray());

                return Ok(result);
            }
        }
    }
}