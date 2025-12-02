#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PlayboxInstaller
{
    public class FinalliseInstallationStage : StageWindowContext
    {
        private string directory => Path.Combine(Application.dataPath, "../DownloadFiles");
        
        public override void Initialize(EditorWindow window, string stageName)
        {
            base.Initialize(window, stageName);
        }

        public override void OnGUI()
        {
            base.OnGUI();
            
            GUILayout.Space(20);

            GUILayout.BeginVertical();

            GUILayout.Label("Installation complete");

            GUILayout.Space(10);
            

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