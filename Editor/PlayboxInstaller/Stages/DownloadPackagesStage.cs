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
        private UnityWebRequest abortRequest = null;

        private float progress = 0;
        private bool isDone = false;
        private bool isDownloading = false;
        private bool isDownloadingCanceled = false;
        private bool isFileExists = false;
        
        private string outputPath = Path.Combine(Application.dataPath, "../DownloadFiles/Firebase.zip");

        public override void OnGUI()
        {
            base.OnGUI();
         
            GUI.enabled = true;
            
            GUILayout.Label("Download Packages");
            
            if (!isDone)
            {
                if (isDownloadingCanceled)
                {
                    EditorGUI.ProgressBar(GUILayoutUtility.GetRect(200, 20),
                        progress,
                        $"❌ Download canceled");
                }
                else
                {
                    EditorGUI.ProgressBar(GUILayoutUtility.GetRect(200, 20),
                        progress,
                        $"{(int)(progress * 100)}%");
                }
            }
            else
            {
                EditorGUI.ProgressBar(GUILayoutUtility.GetRect(200, 20),
                    1,
                    $"✅ Firebase Downloaded 100 %");
            }

            if (isDownloading)
            {
                if (GUILayout.Button("📥 Stop Downloading"))
                {
                    if (abortRequest != null)
                    {
                        isDownloading = false;
                        isDone = false;
                        isDownloadingCanceled = true;
                        abortRequest.Abort();
                    }
                }
            }else
            {
                if (GUILayout.Button("📥 Download Firebase"))
                {
                    _ = Download();
                
                    isDownloading = true;
                    isDownloadingCanceled = false;
                }
            }

            isEnableNextStage = isDone;
            
        }
        
        private async Task Download()
        {
            var url = "https://firebase.google.com/download/unity?hl=ru";
            

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("User-Agent", "Mozilla/5.0 (UnityDownloader)");
            
                request.downloadHandler = new DownloadHandlerFile(outputPath);

                abortRequest = request;

                var webRequestOperation = request.SendWebRequest();
                
                while (!webRequestOperation.isDone)
                {
                    progress = webRequestOperation.progress;
                    
                    if(editorWindow != null)
                        editorWindow.Repaint();
                    
                    await Task.Yield();
                }
                
                isDone = webRequestOperation.isDone && progress >= 0.9f;
                isDownloading = false;
                progress = 0;
            }
        }
    }
}
#endif