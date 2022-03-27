using Microsoft.AspNetCore.Mvc;
using server_api.Services;

namespace server_api.Controllers
{
    /// <summary>
    /// Actions related to the server-api itself
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class Admin : ControllerBase
    {
        /// <summary>
        /// Generate a new API key, invalidates the old one
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        public IActionResult RegenToken()
        {
            AuthService.RegenToken();
            return Ok();
        }

        /// <summary>
        /// Returns ok, needs token
        /// </summary>
        /// <returns></returns>
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("ok");
        }
    }
}