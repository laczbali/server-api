using System.Text.Json.Serialization;

namespace server_api.Services.Git
{
    public static class GitManager
    {
        public static List<string> GetRepos(GitRepoLocation location, bool includePaths = false)
        {
            List<string> repos = new List<string>();

            switch (location)
            {
                case GitRepoLocation.iisRoot:
                    var iisRootFolder = FileManager.GetEnvironmentVariable<string>(new string[] { "paths", "iis-root" });
                    repos = FileManager.GetSubFolders(iisRootFolder);
                    break;

                case GitRepoLocation.gitRoot:
                    var gitRootFolder = FileManager.GetEnvironmentVariable<string>(new string[] { "paths", "git-root" });
                    repos = FileManager.GetSubFolders(gitRootFolder);
                    break;

                default:
                    throw new System.ArgumentException("Unknown location", location.ToString());
            }

            if (includePaths) {
                return repos;
            }

            return repos.Select(x => x.Split('\\').Last()).ToList();
        }

        public static void UpdateRepo(GitRepoLocation location, string repoName)
        {
            // search for desired repo
            var repos = GetRepos(location, includePaths: true);
            var repoPath = repos.Find(x => x.Split('\\').Last() == repoName);

            throw new NotImplementedException();
        }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GitRepoLocation
    {
        gitRoot,
        iisRoot
    }
}