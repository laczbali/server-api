using Microsoft.AspNetCore.Mvc;
using server_api.Services.Git;

namespace server_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ManageRepos
    {

        [HttpPost]
        [Route("[action]")]
        public string UpdateRepo([FromBody] GitRepoLocation location)
        {
            return "ok";
        }
    }
}