using System.Text.Json.Serialization;

namespace server_api.Services.Git
{
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
        /// </summary>
        /// <param name="location">Where is the repo</param>
        /// <param name="repoName">Which repo to update</param>
        /// <param name="type">What other actions are necessary</param>
        /// <param name="args">
        ///     Additional options based on repo type
        ///     <list type="bullet">  
        ///       <item>IIS_SITE: none needed</item>
        ///       <item>SCHEDULED_TASK: name of the task</item>
        ///     </list>
        /// </param>
        public static void UpdateRepo(GitRepoLocation location, string repoName, GitRepoType type, string[]? args)
        {
            // search for desired repo
            var repos = GetRepos(location, includePaths: true);
            var repoPath = repos.Find(x => x.Split('\\').Last() == repoName);

            // pull changes
            ExecuteCommand.ExecuteCMD("git pull", repoPath);

            // make necessary updates
            switch (type)
            {
                case GitRepoType.SCHEDULED_TASK:
                    // restart scheduled task
                    if (args == null) { throw new System.ArgumentException("Scheduled task requires task name as argument"); }
                    var taskname = args[0];
                    ExecuteCommand.ExecutePowerShell($"Stop-ScheduledTask -TaskName {taskname}");
                    ExecuteCommand.ExecutePowerShell($"Start-ScheduledTask -TaskName {taskname}");
                    break;

                case GitRepoType.IIS_SITE:
                    // restart IIS
                    ExecuteCommand.ExecuteCMD("iisreset");
                    break;

                default:
                    break;
            }
        }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GitRepoLocation
    {
        GIT_ROOT,
        IIS_ROOT
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GitRepoType
    {
        SCHEDULED_TASK,
        IIS_SITE,
        OTHER
    }
}