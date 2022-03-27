using Microsoft.AspNetCore.Mvc;

namespace server_api.Controllers
{
    /// <summary>
    /// Website root (/)
    /// </summary>
    [ApiController]
    [Route("/")]
    public class Root : ControllerBase
    {
        /// <summary>
        /// Simply returns "hello"
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Test()
        {
            return Ok("This is the API for blaczko.com");
        }
    }
}