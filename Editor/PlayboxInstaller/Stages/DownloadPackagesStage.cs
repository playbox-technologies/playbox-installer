#if UNITY_EDITOR
using System.IO;
using System.Threading.Tasks;
using Editor.PlayboxInstaller.Stages;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Editor.PlayboxInstaller
{
    public class DownloadPackagesStage : StageWindowContext
    {
        private UnityWebRequestAsyncOperation asyncOperation = null;

        private float progress = 0;
        private bool isDone = false;
        private bool isDownloading = false;

        public override void OnGUI()
        {
            base.OnGUI();
            
            GUILayout.Label("Download Packages");
            
            GUI.enabled = !isDownloading;

            if (GUILayout.Button("📥 Download Firebase"))
            {
                _ = Download();
                isDownloading = true;
            }
            
            GUI.enabled = true;

            if (!isDone)
            {
                EditorGUI.ProgressBar(GUILayoutUtility.GetRect(200, 20),
                    progress,
                    $"{(int)(progress * 100)}%");
            }
            else
            {
                isEnableNextStage = true;
                
                EditorGUI.ProgressBar(GUILayoutUtility.GetRect(200, 20),
                    1,
                    $"Firebase Downloaded 100 %");
            }
        }
        
        private async Task Download()
        {
            var outputPath = Path.Combine(Application.dataPath, "../DownloadFiles/Firebase.zip");
            var url = "https://firebase.google.com/download/unity?hl=ru";
            
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("User-Agent", "Mozilla/5.0 (UnityDownloader)");
            
                request.downloadHandler = new DownloadHandlerFile(outputPath);

                var webRequestOperation = request.SendWebRequest();
                
                while (!webRequestOperation.isDone)
                {
                    progress = webRequestOperation.progress;
                    
                    editorWindow.Repaint();
                    await Task.Yield();
                }
                
                isDone = webRequestOperation.isDone;
                isDownloading = false;
            }
        }
    }
}
#endif