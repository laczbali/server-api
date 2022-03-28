using System.Text.Json.Serialization;

namespace server_api.Services.Git
{
    /// <summary>
    /// Handle git repositories
    /// </summary>
    public static class GitManager
    {
        /// <summary>
        /// Returns the folders in the specified repo directory
        /// </summary>
        /// <param name="location">Which repo directory to check</param>
        /// <param name="includePaths">Should the return values include the full absolute paths</param>
        /// <returns></returns>
        public static List<string> GetRepos(GitRepoLocation location, bool includePaths = false)
        {
            List<string> repos = new List<string>();

            switch (location)
            {
                case GitRepoLocation.IIS_ROOT:
                    var iisRootFolder = FileManager.GetEnvironmentVariable<string>(new string[] { "paths", "iis-root" });
                    repos = FileManager.GetSubFolders(iisRootFolder);
                    break;

                case GitRepoLocation.GIT_ROOT:
                    var gitRootFolder = FileManager.GetEnvironmentVariable<string>(new string[] { "paths", "git-root" });
                    repos = FileManager.GetSubFolders(gitRootFolder);
                    break;

                default:
                    throw new System.ArgumentException("Unknown location", location.ToString());
            }

            if (includePaths)
            {
                return repos;
            }

            return repos.Select(x => x.Split('\\').Last()).ToList();
        }

        /// <summary>
        /// Runs a "git pull" in the specified repo, and makes the necessary updates to hosting services
        /// 
        /// Since the repo params come from the web, THEY ARE TO BE TREATED AS UNSAFE
        /// </summary>
        /// <param name="repo">Where is the repo, how it should be updated, and what is its name</param>
        public static void UpdateRepo(GitRepoDescriptor repo)
        {
            // search for desired repo
            var repos = GetRepos(repo.location, includePaths: true);
            var repoPath = repos.Find(x => x.Split('\\').Last() == repo.name);

            if(repoPath == null)
            {
                throw new System.ArgumentException("Repo not found", repo.name);
            }

            var safeRepoName = repoPath.Split('\\').Last();

            // make necessary updates
            switch (repo.type)
            {
                case GitRepoType.SCHEDULED_TASK:
                    // ExecuteCommand.ExecutePowerShell($"Stop-ScheduledTask -TaskName {safeRepoName}");
                    ExecuteCommand.ExecutePowerShell("..\\..\\..\\..\\scripts\\stop-st.ps1");
                    ExecuteCommand.ExecuteCMD("git pull", repoPath);
                    // ExecuteCommand.ExecutePowerShell($"Start-ScheduledTask -TaskName {safeRepoName}");
                    break;

                case GitRepoType.IIS_SITE:
                    ExecuteCommand.ExecuteCMD("iisreset /stop");
                    ExecuteCommand.ExecuteCMD("git pull", repoPath);
                    // ExecuteCommand.ExecuteCMD("iisreset /start");
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Every info necessary to identify a repository
    /// </summary>
    public class GitRepoDescriptor
    {
        /// <summary>
        /// Where it the repo on the server
        /// </summary>
        /// <value></value>
        public GitRepoLocation location { get; set; }
        /// <summary>
        /// In what context the repo is used (eg hosted by IIS)
        /// </summary>
        /// <value></value>
        public GitRepoType type { get; set; }
        /// <summary>
        /// The name of the repo
        /// </summary>
        /// <value></value>
        public string name { get; set; } = "";
    }

    /// <summary>
    /// Where it the repo on the server
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GitRepoLocation
    {
        /// <summary>
        /// The repo is in the git root folder
        /// </summary>
        GIT_ROOT,
        /// <summary>
        /// The repo is in the IIS root folder
        /// </summary>
        IIS_ROOT
    }

    /// <summary>
    /// In what context the repo is used (eg hosted by IIS)
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GitRepoType
    {
        /// <summary>
        /// The repo is run as a scheduled task
        /// </summary>
        SCHEDULED_TASK,
        /// <summary>
        /// The repo is run as a IIS site
        /// </summary>
        IIS_SITE,
        /// <summary>
        /// The repo is used some other way
        /// </summary>
        OTHER
    }
}