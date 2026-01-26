using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
namespace Editor.PlayboxInstaller.PackageManager
{
        public class GitDependentiesLink
        {
            public string gitRawRef = "https://raw.githubusercontent.com";
            public string gitApiRef = "https://api.github.com";
            public string gitHttpLink = "https://github.com";
            public string gitProjectName = "";
            public string gitOrganization = "";
            public string gitCommitHash = "";
            public string gitFilePath = "";
            public string gitDefaultBranch = "main";
            
            private List<string> branches = new ();
            private int currentBranch = 0;
            
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
                        return $"{gitRawRef}/{gitOrganization}/{gitProjectName}/refs/heads/{GetBranch(currentBranch)}/{gitFilePath}";
                    }
                    else
                    {
                        return $"{gitRawRef}/{gitProjectName}/{gitCommitHash}/refs/heads/{GetBranch(currentBranch)}/{gitFilePath}";
                    }
                }
            }
            
            public string GetPackageGitRef()
            {
                if (!string.IsNullOrEmpty(gitOrganization))
                {
                    return $"{gitHttpLink}/{gitOrganization}/{gitProjectName}.git#{GetBranch(currentBranch)}";
                }
                else
                {
                    return $"{gitHttpLink}/{gitProjectName}/{gitCommitHash}.git#{GetBranch(currentBranch)}";
                }
            }

            public void RegisterBranch(string branch)
            {
                branches.Add(branch);
            }

            public string[] GetBranchArray()
            {
                return branches.ToArray();
            }

            public int GetCurrentBranch()
            {
                return currentBranch;
            }
            
            public void SetCurrentBranch(int branch) => currentBranch = Mathf.Clamp(branch,0, branches.Count - 1);

            public void ResetToDefaultBranch()
            {
                currentBranch = Mathf.Clamp(branches.IndexOf(gitDefaultBranch), 0, branches.Count - 1);
            }

            private string GetBranch(int branch)
            {
                if(branch < 0 || branch >= branches.Count)
                    throw new ArgumentException("Invalid branch");
                
                return branches[branch];
            }

            public string GetBranchesURL()
            {
                var url = $"{gitApiRef}/repos/{gitOrganization}/{gitProjectName}/branches";

                return url;
            }
        }
}
#endif