#if UNITY_EDITOR
using PlayboxInstaller;
using UnityEditor;
using UnityEngine;

namespace Editor.PlayboxInstaller.PackageManager
{
    public class PlayboxGitToken : EditorWindow
    {
        string _gitToken = "";
        
        [MenuItem("Playbox/Installer/Set GitHub Token")]
        private static void UnpackArhivesMenu()
        {
            GetWindow<PlayboxGitToken>("Github Token Window");
        }

        private void OnGUI()
        {
            PlayboxLayout.VerticalLayout(() =>
            { 
                PlayboxLayout.HorizontalLayout((() =>
                {
                    GUILayout.Label("Github Token", EditorStyles.boldLabel,GUILayout.Width(100));
                    _gitToken = EditorGUILayout.TextArea(_gitToken,GUILayout.Width(450));
                    
                }));
                
                PlayboxLayout.HorizontalLayout((() =>
                {
                    if (GUILayout.Button("Set Token",GUILayout.Width(200)))
                    {
                        EditorPrefs.SetString("GithubToken", _gitToken);
                    }
                }));
                
                bool hasToken = EditorPrefs.HasKey("GithubToken");
                
                PlayboxLayout.HorizontalLayout((() =>
                {
                    GUILayout.Label("Current Token", EditorStyles.boldLabel,GUILayout.Width(100));
                    
                    if (hasToken)
                    {
                        var token = EditorPrefs.GetString("GithubToken");
                        
                        GUILayout.Label(token, EditorStyles.boldLabel,GUILayout.Width(450));
                    }
                    else
                    {
                        GUILayout.Label("none", EditorStyles.boldLabel,GUILayout.Width(450));   
                    }
                }));
                
                PlayboxLayout.HorizontalLayout(() =>
                {
                    if (GUILayout.Button("Copy Token",GUILayout.Width(100)))
                    {
                        if (hasToken)
                        {
                            var token = EditorPrefs.GetString("GithubToken");
                            
                            GUIUtility.systemCopyBuffer = token;
                            
                            Debug.Log($"Copy to buffer {token}");
                        }
                    }

                    if (GUILayout.Button("Clear Token",GUILayout.Width(100)))
                    {
                        EditorPrefs.DeleteKey("GithubToken");
                    }
                });
                
            });
        }
    }
}
#endif