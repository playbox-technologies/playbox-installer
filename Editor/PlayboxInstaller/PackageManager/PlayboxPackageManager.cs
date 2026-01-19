#if UNITY_EDITOR

using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PlayboxInstaller;
using UnityEditor;
using UnityEngine;

namespace Editor.PlayboxInstaller.PackageManager
{
    public partial class PlayboxPackageManager : EditorWindow
    {
        private string playbox_actual_version = "0.0.1";
        private string playbox_current_version = "0.0.1";
        
        
        [MenuItem("Playbox/Installer/Package Manager")]
        private static void UnpackArhivesMenu()
        {
            GetWindow<PlayboxPackageManager>("Playbox Package Manager");
        }

        private async void CreateGUI()
        {
            GitDependentiesLink playboxAcrualRepo = new GitDependentiesLink();
            playboxAcrualRepo.isHashedLock = false;
            playboxAcrualRepo.gitBranch = "main";
            playboxAcrualRepo.gitProjectName = "playbox-sdk";
            playboxAcrualRepo.gitOrganization = "playbox-technologies";
            playboxAcrualRepo.gitFilePath = "package.json";
            
            var playboxVersion =  await GetActualPlayboxVersion(playboxAcrualRepo.GetRawGitRef());
            
            playbox_actual_version = playboxVersion;

            var packageVersion = LockedRepositoryHelper.GetDependencyVersion("playbox");

            GitDependentiesLink playboxCurrentRepo = new GitDependentiesLink();
            playboxCurrentRepo.isHashedLock = true;
            playboxCurrentRepo.gitCommitHash = packageVersion.hash;
            playboxCurrentRepo.gitProjectName = "playbox-sdk";
            playboxCurrentRepo.gitOrganization = "playbox-technologies";
            playboxCurrentRepo.gitFilePath = "package.json";
            
            playbox_current_version = await GetActualPlayboxVersion(playboxCurrentRepo.GetRawGitRef());
        }

        private void OnGUI()
        {
            PlayboxLayout.HorizontalLayout(() =>
            {
                GUILayout.Label("Install Playbox");
                
                GUILayout.Label("Actual version");
                GUILayout.Label(playbox_actual_version);
                
                GUILayout.Label("Current version");
                GUILayout.Label(playbox_current_version);
                
                if (GUILayout.Button("Install"))
                {
                    Debug.Log("Installing Playbox");
                }
            });
        }

        private async Task<string> GetActualPlayboxVersion(string url = "")
        {
            var res = await HttpHelper.GetAsync(
                url);

            var packageJson = JObject.Parse(res.Body);
            
            return packageJson["version"]?.ToString();
        }
    }
}

#endif