using System.Collections.Generic;
using System.Threading.Tasks;
using PlayboxInstaller;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

namespace Editor.PlayboxInstaller.PackageManager
{
    public static class PlayboxPackageRegister
    {
        private static List<GitDependentiesLink> _dependentiesLinks = new();

        public static List<GitDependentiesLink> DependentiesLinks
        {
            get => _dependentiesLinks;
            set => _dependentiesLinks = value;
        }

        public static async void Register()
        {
            string repositories = await HttpGET("https://api.github.com/orgs/playbox-technologies/repos?type=public");
            
            JArray repositoriesArray = JArray.Parse(repositories);

            foreach (var repository in repositoriesArray)
            {
                GitDependentiesLink dependentiesLink = new();
                
                dependentiesLink.gitOrganization = repository["owner"]?["login"]?.Value<string>();
                dependentiesLink.gitProjectName = repository["name"]?.Value<string>();
                dependentiesLink.gitDefaultBranch = repository["default_branch"]?.Value<string>();
                dependentiesLink.isHashedLock = false;

                string branchesURL = dependentiesLink.GetBranchesURL();

                var branchesBody = JArray.Parse(await HttpGET(branchesURL));

                foreach (var branch in branchesBody)
                {
                    
                    string branchName = branch["name"]?.Value<string>();
                    
                    dependentiesLink.RegisterBranch(branchName);
                }
                
                dependentiesLink.ResetToDefaultBranch();
                
                _dependentiesLinks.Add(dependentiesLink);
            }
        }

        private static async Task<string> HttpGET(string url) => (await HttpHelper.GetAsync(url)).Body;
    }
}