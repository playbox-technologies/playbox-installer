#if UNITY_EDITOR
using System;
using System.IO;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

namespace PlayboxInstaller
{
    [Serializable]
    public class PlayboxRepository
    {
        public enum RepositoryType
        {
            Registry,
            Git,
            Local
        }
        
        public string hash;
        public string url;
        public RepositoryType repositoryType;

        public override string ToString()
        {
            var json =JsonUtility.ToJson(this);
            
            return json;
        }
    }

    public class LockedRepositoryHelper
    {
        private static string manifestPath => Path.Combine(Application.dataPath, "../Packages/packages-lock.json");
        
        public static PlayboxRepository GetDependencyVersion(string packageName)
        {
            PlayboxRepository repository = new PlayboxRepository();

            repository.repositoryType = PlayboxRepository.RepositoryType.Registry;
            
            JObject json = JObject.Parse(File.ReadAllText(manifestPath));

            var dependencies = json["dependencies"];
            
            Debug.Log(dependencies?.ToString());
            
            var package = (JObject)dependencies?[packageName];
            
            Debug.Log(package?.ToString());
            
            var version = package?["version"];
            
            string packageVersion = version?.Value<string>();
            
            if (string.IsNullOrEmpty(packageVersion))
                packageVersion = "not installed";

            if (package["source"]?.Value<string>() == "git")
            {
                repository.repositoryType = PlayboxRepository.RepositoryType.Git;
                
                int tokenIndex = packageVersion.IndexOf("#", StringComparison.Ordinal);

                if (tokenIndex >= 0)
                {
                 
                    var uri = packageVersion.Substring(0, tokenIndex);
                    
                }

                repository.hash = package?["hash"]?.Value<string>();
            }
            
            return repository;
        }
    }
}

#endif