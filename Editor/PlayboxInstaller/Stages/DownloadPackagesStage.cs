#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace PlayboxInstaller
{
    public class DownloadPackagesStage : StageWindowContext
    {
        private UnityWebRequest _abortRequest = null;

        private float _progress;
        private bool _isDone;
        private bool _isDownloading;
        private bool _isDownloadingCanceled;
        
        private List<UnityPackageData> _packagesData = new();

        private string outputPath = Path.Combine(Application.dataPath, "../DownloadFiles/Firebase.zip");

        public override void Initialize(EditorWindow window)
        {
            base.Initialize(window);

            _packagesData = FirebaseArhivesData.UnpackArhives();
        }

        public override void OnGUI()
        {
            base.OnGUI();

            GUI.enabled = true;

            GUILayout.Label("Download Packages");

            if (!_isDone)
            {
                if (_isDownloadingCanceled)
                {
                    EditorGUI.ProgressBar(GUILayoutUtility.GetRect(200, 20),
                        _progress,
                        $"❌ Download canceled");
                }
                else
                {
                    EditorGUI.ProgressBar(GUILayoutUtility.GetRect(200, 20),
                        _progress,
                        $"{(int)(_progress * 100)}%");
                }
            }
            else
            {
                EditorGUI.ProgressBar(GUILayoutUtility.GetRect(200, 20),
                    1,
                    $"✅ Firebase Downloaded 100 %");
            }

            if (_isDownloading)
            {
                if (GUILayout.Button("📥 Stop Downloading"))
                {
                    _isDownloading = false;
                    _isDone = false;
                    _isDownloadingCanceled = true;
                    
                    _abortRequest?.Abort();
                    _packagesData = FirebaseArhivesData.UnpackArhives();
                }
            }
            else
            {
                if (GUILayout.Button("📥 Download Firebase"))
                {
                    _ = Download();

                    _isDownloading = true;
                    _isDownloadingCanceled = false;
                    _packagesData = FirebaseArhivesData.UnpackArhives();
                }
            }
            
            isEnableNextStage = _isDone;

            if (_packagesData != null)
            {
                isEnableNextStage = _packagesData.Count > 1;
            }
        }

        private async Task Download()
        {
            var url = "https://firebase.google.com/download/unity?hl=ru";

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("User-Agent", "Mozilla/5.0 (UnityDownloader)");

                request.downloadHandler = new DownloadHandlerFile(outputPath);

                _abortRequest = request;

                var webRequestOperation = request.SendWebRequest();
                
                while (!webRequestOperation.isDone)
                {
                    _progress = webRequestOperation.progress;

                    if (editorWindow != null)
                        editorWindow.Repaint();
                    
                    await Task.Yield();
                }

                _isDone = webRequestOperation.isDone && _progress >= 0.9f;
                _isDownloading = false;
                _progress = 0;
                _packagesData = FirebaseArhivesData.UnpackArhives();
            }
        }
    }
}
#endif