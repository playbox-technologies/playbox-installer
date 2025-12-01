#if UNITY_EDITOR
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace PlayboxInstaller
{
    public class InstallPlayboxStage : StageWindowContext
    {
        private const string PlayboxPackageName = "";
        private const string playbox_url = "https://github.com/playbox-technologies/playbox-sdk.git#";
        private const string playbox_branch = "main";
        
        private JObject deps;
        
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

        public override void Initialize(EditorWindow window, string stageName)
        {
            base.Initialize(window, stageName);
            
            deps = ManifestData.GetDependencies();
        }

        public override void OnGUI()
        {
            base.OnGUI();
            
            GUILayout.Space(5);

            GUILayout.Label("Install Dependenties");

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

            GUILayout.Space(10);

            GUI.enabled = !isEnableNextStage;

            if (GUILayout.Button("Install Playbox"))
            {
                var request = Client.Add($"{playbox_url}{playbox_branch}");

                EditorApplication.update += Update;

                void Update()
                {
                    if (request.IsCompleted)
                    {
                        if (request.Status == StatusCode.Success)
                        {
                            Debug.Log("Playbox SDK Installed");
                        }
                        else if (request.Status == StatusCode.Failure)
                        {
                            Debug.Log("Playbox SDK Installation failed");
                        }

                        EditorApplication.update -= Update;
                    }
                }
            }

            GUI.enabled = true;
        }
    }
}

#endif