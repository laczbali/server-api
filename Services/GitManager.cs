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

            // pull changes
            ExecuteCommand.ExecuteCMD("git pull", repoPath);

            // make necessary updates
            switch (repo.type)
            {
                case GitRepoType.SCHEDULED_TASK:
                    // restart scheduled task
                    ExecuteCommand.ExecutePowerShell($"Stop-ScheduledTask -TaskName {safeRepoName}");
                    ExecuteCommand.ExecutePowerShell($"Start-ScheduledTask -TaskName {safeRepoName}");
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

    public class GitRepoDescriptor
    {
        public GitRepoLocation location { get; set; }
        public GitRepoType type { get; set; }
        public string name { get; set; } = "";
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