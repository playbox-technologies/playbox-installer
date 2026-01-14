#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace PlayboxInstaller
{
    public class FinalliseInstallationStage : StageWindowContext
    {
        private string directory => Path.Combine(Application.dataPath, "../DownloadFiles");
        
        private Dictionary<string, string> packagesToAdd = new ()
        {
            { "appsflyer-unity-plugin", "https://github.com/AppsFlyerSDK/appsflyer-unity-plugin.git#upm" },
            { "com.devtodev.sdk.analytics", "https://github.com/devtodev-analytics/package_Analytics.git" },
            { "com.devtodev.sdk.analytics.google", "https://github.com/devtodev-analytics/package_Google.git" },
            { "com.google.external-dependency-manager", "1.2.186" },
            { "com.applovin.mediation.ads", "8.3.1" },
            { "com.google.ads.mobile", "10.3.0" },
            { "com.unity.ads.ios-support", "1.0.0" }
        };
        
        private JObject deps;
        
        public override void Initialize(EditorWindow window, string stageName)
        {
            base.Initialize(window, stageName);
            deps = ManifestData.GetDependencies();
        }

        public override void OnGUI()
        {
            base.OnGUI();
            
            GUILayout.Space(20);

            GUILayout.BeginVertical();

            GUILayout.Label("Installation complete");
            
            GUILayout.Space(10);

            GUILayout.BeginVertical();
            
            foreach (var item in packagesToAdd)
            {
                GUILayout.BeginHorizontal();
                
                if (deps != null && deps[item.Key] == null)
                {
                    GUILayout.Label($"📦❌ {item.Key}");
                    GUILayout.Label("not installed",GUILayout.ExpandWidth(false));
                }
                else
                {
                    GUILayout.Label($"📦✅ {item.Key}");
                    GUILayout.Label("installed",GUILayout.ExpandWidth(false));
                }
                
                GUILayout.EndHorizontal();
                
                GUILayout.Space(2);
            }
            
            GUILayout.Space(2);
            
            GUILayout.EndVertical();
            

            if (GUILayout.Button("Clear temporary files"))
            {
                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, true);
                    
                    Debug.Log($"Directory {directory}  deleted");
                }
                else
                {
                    Debug.Log($"The directory {directory} has already been deleted.");
                }

            }

            GUILayout.EndVertical();
        }
    }
}
#endif