#if UNITY_EDITOR
using System.IO;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

namespace PlayboxInstaller
{
    public class LockedRepositoryHelper
    {
        private static string manifestPath => Path.Combine(Application.dataPath, "../Packages/packages-lock.json");
        
        public static string GetDependencyVersion(string packageName)
        {
            JObject json = JObject.Parse(File.ReadAllText(manifestPath));

            var dependencies = json["dependencies"];
            
            Debug.Log(dependencies?.ToString());
            
            var package = (JObject)dependencies?[packageName];
            
            Debug.Log(package?.ToString());
            
            var version = package?["version"];
            
            string packageVersion = version?.Value<string>();
            
            if (string.IsNullOrEmpty(packageVersion))
                packageVersion = "not installed";

            if (package["source"]?.ToString() == "git")
            {
                Debug.Log("Is is git");
            }

            return packageVersion;
        }
    }
}

#endif