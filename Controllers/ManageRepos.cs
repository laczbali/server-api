using Microsoft.AspNetCore.Mvc;
using server_api.Services.Git;

namespace server_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ManageRepos : ControllerBase
    {

        [HttpPost]
        [Route("[action]")]
        public IActionResult UpdateRepo([FromBodyAttribute] GitRepoDescriptor repoInfo)
        {
            try
            {
                GitManager.UpdateRepo(repoInfo);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}