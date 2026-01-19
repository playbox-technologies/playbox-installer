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
        private static string LockedFilePath => Path.Combine(Application.dataPath, "../Packages/packages-lock.json");
        
        private static JObject _lockedRepositoryJson;
        private static JObject _dependentiesJson;
        
        private static DateTime _lastEditingTime = DateTime.MinValue;
        private static string _fileData;

        private static string GetLockedFile()
        {
            FileInfo fileInfo = new FileInfo(LockedFilePath);

            if (_lastEditingTime < fileInfo.LastWriteTimeUtc)
            {
                _lastEditingTime = fileInfo.LastWriteTime;
                
                _fileData = File.ReadAllText(LockedFilePath);
            }

            return _fileData;
        }
        
        public static PlayboxRepository GetDependencyVersion(string packageName)
        {
            PlayboxRepository repository = new PlayboxRepository();

            repository.repositoryType = PlayboxRepository.RepositoryType.Registry;
            
            _lockedRepositoryJson = JObject.Parse(GetLockedFile());

            _dependentiesJson = (JObject)_lockedRepositoryJson["dependencies"];
            
            Debug.Log(_dependentiesJson?.ToString());
            
            var package = (JObject)_dependentiesJson?[packageName];
            
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