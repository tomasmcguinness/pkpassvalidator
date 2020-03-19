using Microsoft.AspNetCore.Mvc;
using PassValidator.API.Models;
using PassValidator.Lib.Validation;
using System;

namespace PassValidator.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValidationController : ControllerBase
    {
        [HttpPost]
        public IActionResult Validate([FromBody] PkPass pkPass)
        {
            try
            {
                return Ok(new Validator().Validate(
                System.Convert.FromBase64String(pkPass.EncodedBytes)));
            }
            catch(Exception ex) when (ex is FormatException || ex is ArgumentNullException)
            {
                return BadRequest(new Error("Payload missing or invalid. Check if base64 encoded."));
            }
            
        }
    }
}
