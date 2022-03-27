using Microsoft.AspNetCore.Mvc;
using server_api.Services.Git;

namespace server_api.Controllers
{
    /// <summary>
    /// Manage git repositories handled by the server
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ManageRepos : ControllerBase
    {
        /// <summary>
        /// Update a git repository:
        /// Pull latest changes from remote,
        /// Perform type-dependent post actions
        /// </summary>
        /// <param name="repoInfo"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public IActionResult UpdateRepo([FromBodyAttribute] GitRepoDescriptor repoInfo)
        {
            try
            {
                // TODO: scheduled tasks definately don't get restarted
                // TODO: iis probably doesnt get reastarted, based on the time it takes
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