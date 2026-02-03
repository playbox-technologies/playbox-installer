#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace PlayboxInstaller
{
    [Serializable]
    public class PlayboxPackageInfo
    {
        public enum RepositoryType
        {
            Registry,
            Git,
            Local
        }


        public bool Exist = false;
        public string hash;
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

        private static void GetLockedFile()
        {
            FileInfo fileInfo = new FileInfo(LockedFilePath);

            if (_lastEditingTime < fileInfo.LastWriteTimeUtc)
            {
                _lastEditingTime = fileInfo.LastWriteTime;
                
                _fileData = File.ReadAllText(LockedFilePath);
                
                _lockedRepositoryJson = JObject.Parse(_fileData);
                _dependentiesJson = (JObject)_lockedRepositoryJson["dependencies"];
            }
        }

        private static PlayboxPackageInfo GetPackageInfo(string packageName, in JObject dependencies)
        {
            PlayboxPackageInfo repository = new PlayboxPackageInfo();

            repository.repositoryType = PlayboxPackageInfo.RepositoryType.Registry;

            if (!dependencies.TryGetValue(packageName, out var dependency))
            {
                repository.Exist = false;
                
                return repository;
            }

            var package = (JObject)dependency;
            
            var source = package?["source"]?.Value<string>();

            if (source == "git")
            {
                repository.hash = package?["hash"]?.Value<string>();

                repository.repositoryType = PlayboxPackageInfo.RepositoryType.Git;
            }
            
            
            
            return repository;
        }

        public static List<PlayboxPackageInfo> GetDependencyVersion(List<string> packageList)
        {
            GetLockedFile();

            List<PlayboxPackageInfo> packageInfos = new List<PlayboxPackageInfo>();
            
            foreach (var package in packageList)
            {
                packageInfos.Add(GetPackageInfo(package, in _dependentiesJson));
            }

            return packageInfos;
        }
    }
}

#endif