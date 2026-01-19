#if UNITY_EDITOR
namespace Editor.PlayboxInstaller.PackageManager
{
    public partial class PlayboxPackageManager
    {
        public class GitDependentiesLink
        {
            public string gitRawRef = "https://raw.githubusercontent.com";
            public string gitApiRef = "https://api.github.com";
            public string gitProjectName = "";
            public string gitOrganization = "";
            public string gitBranch = "";
            public string gitCommitHash = "";
            public string gitFilePath = "";
            
            public bool isHashedLock = false;

            public string GetRawGitRef()
            {
                if (isHashedLock)
                {
                    if (!string.IsNullOrEmpty(gitOrganization))
                    {
                        return $"{gitRawRef}/{gitOrganization}/{gitProjectName}/{gitCommitHash}/{gitFilePath}";
                    }
                    else
                    {
                        return $"{gitRawRef}/{gitProjectName}/{gitCommitHash}/{gitFilePath}";
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(gitOrganization))
                    {
                        return $"{gitRawRef}/{gitOrganization}/{gitProjectName}/refs/heads/{gitBranch}/{gitFilePath}";
                    }
                    else
                    {
                        return $"{gitRawRef}/{gitProjectName}/{gitCommitHash}/refs/heads/{gitBranch}/{gitFilePath}";
                    }
                }
            }

            public string GetBranchesURL()
            {
                var url = $"{gitApiRef}/repos/{gitOrganization}/{gitProjectName}/branches";

                return url;
            }
        }
    }
}
#endif