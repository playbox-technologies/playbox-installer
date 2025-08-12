#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlayboxInstaller
{
    public class InstallFirebaseStage : StageWindowContext
    {
        private List<UnityPackageData> unityPackages = new();

        public override void Initialize(EditorWindow window)
        {
            base.Initialize(window);
            unityPackages = FirebaseArhivesData.UnpackArhives();
        }

        public override void OnGUI()
        {
            base.OnGUI();
            
            GUILayout.Space(20);
            
            GUILayout.BeginVertical();
            
            GUILayout.Label("Install Firebase packages");
            
            GUILayout.Space(10);
            
            foreach (var item in unityPackages)
            {
                GUILayout.Space(5);
                
                GUILayout.BeginHorizontal();
                
                GUILayout.Space(10);
                
                GUILayout.Label(item.packageName);
                
                item.isImporting = GUILayout.Toggle(item.isImporting, "Importing",GUILayout.ExpandWidth(false));
                
                GUILayout.Space(10);
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Install Arhives")) 
            {
                FirebaseArhivesData.InstallArhives(unityPackages);
                
                isEnableNextStage = true;
            }

            GUILayout.EndVertical();
        }
    }
}
#endif