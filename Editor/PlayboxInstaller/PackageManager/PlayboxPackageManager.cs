#if UNITY_EDITOR

using System;
using PlayboxInstaller;
using UnityEditor;
using UnityEngine;

namespace Editor.PlayboxInstaller.PackageManager
{
    public class PlayboxPackageManager : EditorWindow
    {
        [MenuItem("Playbox/Installer/Package Manager")]
        private static void UnpackArhivesMenu()
        {
            GetWindow<PlayboxPackageManager>("Playbox Package Manager");
        }


        private void CreateGUI()
        {
            PlayboxPackageRegister.Initialize();
            PlayboxPackageRegister.Register();
        }

        private void OnGUI()
        {
            if (PlayboxPackageRegister.DependentiesLinks.Count == 0)
            {
                PlayboxLayout.VerticalLayout(() =>
                {
                    GUILayout.Label("Updating Playbox Repositories ...");
                    
                });   
            }
            else
            {
                PlayboxLayout.VerticalLayout(() =>
                {
                    DateTime time = PlayboxPackageRegister.LastUpdate;
                    DateTime nextTime = time.AddMinutes(PlayboxPackageRegister.UpdateRateInMinutes);

                    PlayboxLayout.HorizontalLayout(() =>
                    {
                        GUILayout.Label($"Last Update : {time.Hour:00}:{time.Minute:00}");
                        GUILayout.Label($"Next Update : {nextTime.Hour:00}:{nextTime.Minute:00}");

                        if (GUILayout.Button("Update Now"))
                        {
                            PlayboxPackageRegister.UpdateNow();
                        }
                    });
                }); 
            }
            
            foreach (var dependentiesLink in PlayboxPackageRegister.DependentiesLinks)
            {
                PlayboxLayout.HorizontalLayout(() =>
                {
                    GUILayout.Label(dependentiesLink.gitProjectName, GUILayout.ExpandWidth(false), GUILayout.Width(300));

                    int branch = EditorGUILayout.Popup("", dependentiesLink.GetCurrentBranch(),dependentiesLink.GetBranchArray());
                
                    dependentiesLink.SetCurrentBranch(branch);
                    
                    if (GUILayout.Button("Install"))
                    {
                        Debug.Log($"Installing {dependentiesLink.GetPackageGitRef()}");
                    }
                });
                
                GUILayout.Space(10);
            }
        }
    }
}

#endif