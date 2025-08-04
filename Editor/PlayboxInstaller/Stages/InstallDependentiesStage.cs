#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Editor.PlayboxInstaller.Stages;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Editor.PlayboxInstaller
{
    public class InstallDependentiesStage : StageWindowContext
    {
        private static string ManifestPath => Path.Combine(Application.dataPath, "../Packages/manifest.json");
        
        private Dictionary<string, string> packagesToAdd = new ()
        {
            { "com.appsflyer.unity", "https://github.com/AppsFlyerSDK/appsflyer-unity-plugin.git#upm" },
            {
                "com.playbox.facebooksdk",
                "https://github.com/playbox-technologies/playbox-dependenties.git?path=facebook-sdk#main"
            },
            { "com.devtodev.sdk.analytics", "https://github.com/devtodev-analytics/package_Analytics.git" },
            { "com.devtodev.sdk.analytics.google", "https://github.com/devtodev-analytics/package_Google.git" },
            { "com.google.external-dependency-manager", "1.2.186" },
            { "com.applovin.mediation.ads", "8.3.1" },
            { "com.google.ads.mobile", "10.3.0" },
            { "com.unity.ads.ios-support", "1.0.0" }
        };

        public override void Initialize(EditorWindow window)
        {
            base.Initialize(window);
            
            
        }

        public override void OnGUI()
        {
            base.OnGUI();

            GUILayout.Label("Install Dependenties");
            
            if (GUILayout.Button("Install Dependencies"))
            {
                AddPackagesToManifest();
                isEnableNextStage = true;
            }
        }

        private JObject GetDependencies()
        {
            if (!File.Exists(ManifestPath))
            {
                Debug.LogError("manifest.json not found!");
                
                throw new FileNotFoundException("manifest.json not found!");
            }
            
            var manifestJson = JObject.Parse(File.ReadAllText(ManifestPath));

            var dependencies = (JObject)manifestJson["dependencies"];
            
            return dependencies;
        }

        private void AddPackagesToManifest()
        {
            if (!File.Exists(ManifestPath))
            {
                Debug.LogError("manifest.json not found!");
                return;
            }

            var manifestJson = JObject.Parse(File.ReadAllText(ManifestPath));

            var dependencies = (JObject)manifestJson["dependencies"];

            foreach (var item in packagesToAdd)
            {
                if (dependencies != null && dependencies[item.Key] == null)
                {
                    dependencies[item.Key] = item.Value;
                }
                else
                {
                    if (dependencies != null) dependencies[item.Key] = item.Value;
                    Debug.Log($"Package {item.Key} already exists!");
                }
            }

            if (manifestJson["scopedRegistries"] == null)
            {
                manifestJson["scopedRegistries"] = new JArray();
            }

            var registries = (JArray)manifestJson["scopedRegistries"];

            if (!hasRegistry("applovin", registries))
            {
                registries = AddToRegistry(registries,
                    "AppLovin MAX Unity",
                    "https://unity.packages.applovin.com/",
                    new JArray("com.applovin.mediation.ads",
                        "com.applovin.mediation.adapters",
                        "com.applovin.mediation.dsp"));
            }

            if (!hasRegistry("openupm", registries))
            {
                registries = AddToRegistry(registries,
                    "package.openupm.com",
                    "https://package.openupm.com",
                    new JArray("com.google"));
            }


            manifestJson["scopedRegistries"] = registries;
            manifestJson["dependencies"] = dependencies;

            File.WriteAllText(ManifestPath, manifestJson.ToString(Newtonsoft.Json.Formatting.Indented));

            AssetDatabase.Refresh();
            Client.Resolve();
        }

        static JArray AddToRegistry(JArray registries, string name, string url, JArray scopes)
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

        static bool hasRegistry(string registryName, JArray array)
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
    }
}
#endif