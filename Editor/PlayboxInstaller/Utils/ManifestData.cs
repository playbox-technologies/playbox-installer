#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace PlayboxInstaller
{
    public static class ManifestData
    {
        private static string manifestPath => Path.Combine(Application.dataPath, "../Packages/manifest.json");

        public static void AddPackagesToManifest(Dictionary<string, string> packagesToAdd)
        {
            if (!File.Exists(manifestPath))
            {
                Debug.LogError("manifest.json not found!");
                return;
            }
            
            var manifestText = File.ReadAllText(manifestPath);

            if (string.IsNullOrEmpty(manifestText))
            {
                Debug.LogError("manifest.json is empty!");
                return;
            }

            var manifestJson = JObject.Parse(manifestText);

            var dependencies = (JObject)manifestJson["dependencies"];

            if (dependencies == null)
            {
                Debug.LogError("manifest.json dependenties is empty!");
            }

            foreach (var item in packagesToAdd)
            {
                dependencies![item.Key] = item.Value;
            }

            if (manifestJson["scopedRegistries"] == null)
            {
                manifestJson["scopedRegistries"] = new JArray();
            }
            
            var scopedRegistries = (JArray)manifestJson["scopedRegistries"];
            
            var hasAppLovinRegistries = HasRegistry("applovin", scopedRegistries);
            var hasOpenUPMRegistries = HasRegistry("openupm", scopedRegistries);


            if (!hasAppLovinRegistries)
            {
                scopedRegistries = AddToRegistry(scopedRegistries,
                    "AppLovin MAX Unity",
                    "https://unity.packages.applovin.com/",
                    new JArray("com.applovin.mediation.ads",
                        "com.applovin.mediation.adapters",
                        "com.applovin.mediation.dsp"));
            }
            
            if (!hasOpenUPMRegistries)
            {
                scopedRegistries = AddToRegistry(scopedRegistries,
                    "package.openupm.com",
                    "https://package.openupm.com",
                    new JArray("com.google"));
            }

            manifestJson["scopedRegistries"] = scopedRegistries;
            manifestJson["dependencies"] = dependencies;
            
            File.WriteAllText(manifestPath, manifestJson.ToString(Newtonsoft.Json.Formatting.Indented));
            
            AssetDatabase.Refresh();
            Client.Resolve();
            
        }
        
        public static JObject GetDependencies()
        {
            if (!File.Exists(manifestPath))
            {
                Debug.LogError("manifest.json not found!");
                
                throw new FileNotFoundException("manifest.json not found!");
            }
            
            var manifestJson = JObject.Parse(File.ReadAllText(manifestPath));

            var dependencies = (JObject)manifestJson["dependencies"];
            
            return dependencies;
        }
        
        public static bool PackageExists(this JObject dependencies,string packageName)
        {
            if (dependencies == null)
                return false;

            if (dependencies.ContainsKey(packageName))
            {
                return true;
            }
            
            return false;
        }

        public static bool PackageExists(string packageName)
        {
            var dependencies = GetDependencies();
            
            if (dependencies == null)
                return false;

            if (dependencies.ContainsKey(packageName))
            {
                return true;
            }
            
            return false;
        }
        
        public static bool HasRegistry(string registryName, JArray array)
        {
            if (array == null)
                return false;

            foreach (var item in array)
            {
                if (item["url"] != null && item["url"].ToString().Contains(registryName))
                {
                    return true;
                }
            }

            return false;
        }
        
       public static JArray AddToRegistry(JArray registries, string name, string url, JArray scopes)
        {
            var newResigtry = new JObject
            {
                ["name"] = name,
                ["url"] = url,
                ["scopes"] = scopes
            };

            registries.Add(newResigtry);

            return registries;
        }
    }
}
#endif