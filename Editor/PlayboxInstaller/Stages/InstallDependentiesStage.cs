#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace PlayboxInstaller
{
    public class InstallDependentiesStage : StageWindowContext
    {
        private static string ManifestPath => Path.Combine(Application.dataPath, "../Packages/manifest.json");
        
        private JObject deps;
        
        private static readonly Dictionary<string, string> packagesToAdd = new ()
        {
            { "appsflyer-unity-plugin", "https://github.com/AppsFlyerSDK/appsflyer-unity-plugin.git#upm" },
            { "com.devtodev.sdk.analytics", "https://github.com/devtodev-analytics/package_Analytics.git" },
            { "com.devtodev.sdk.analytics.google", "https://github.com/devtodev-analytics/package_Google.git" },
            { "com.google.external-dependency-manager", "1.2.186" },
            { "com.applovin.mediation.ads", "8.3.1" },
            { "com.google.ads.mobile", "10.3.0" },
            { "com.unity.ads.ios-support", "1.0.0" }
        };

        public override void Initialize(EditorWindow window, string stageName)
        {
            base.Initialize(window, stageName);

            deps = ManifestData.GetDependencies();
        }

        public override void OnGUI()
        {
            base.OnGUI();

            int NextStageIndex = 0;

            GUILayout.Space(5);
            
            GUILayout.Label("Install Dependenties");
            
            GUILayout.Space(10);

            GUILayout.BeginVertical();
            
            foreach (var item in packagesToAdd)
            {
                string packageName = item.Key;
                bool hasPackageExist = deps.PackageExists(packageName);
                string packageNameLabel = hasPackageExist ? $"📦✅ {item.Key}" : $"📦❌ {packageName}";
                string installedLabel = $"{(hasPackageExist ? "": "not")} installed";
                
                NextStageIndex += hasPackageExist ? 1 : 0;
                
                GUILayout.BeginHorizontal();
                
                GUILayout.Label(packageNameLabel);
                GUILayout.Label(installedLabel,GUILayout.ExpandWidth(false));
                
                GUILayout.EndHorizontal();
                
                GUILayout.Space(2);
            }
            
            GUILayout.EndVertical();
            
            GUILayout.Space(10);

            isEnableNextStage = NextStageIndex >= packagesToAdd.Count;

            GUI.enabled = !isEnableNextStage;
            
            if (GUILayout.Button("Install Dependencies"))
            {
                ManifestData.AddPackagesToManifest(packagesToAdd);
            }
            
            GUI.enabled = true;
        }
    }
}
#endif