#if UNITY_EDITOR

using System;
using System.Collections.Generic;
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
            var str = await GetPackageInfo("https://api.github.com/orgs/playbox-technologies/repos?type=public&sort=updated&direction=desc&per_page=100&page=1");
            
            Debug.Log(str);
            
            GitDependentiesLink playboxAcrualRepo = new GitDependentiesLink();
            playboxAcrualRepo.isHashedLock = false;
            playboxAcrualRepo.gitBranch = "main";
            playboxAcrualRepo.gitProjectName = "playbox-sdk";
            playboxAcrualRepo.gitOrganization = "playbox-technologies";
            playboxAcrualRepo.gitFilePath = "package.json";
            
            var playboxString =  await GetPackageInfo(playboxAcrualRepo.GetRawGitRef());
            
            var packageData = JObject.Parse(playboxString);
            
            playbox_actual_version = packageData?["version"]?.ToString();

            var packageVersion = LockedRepositoryHelper.GetDependencyVersion(new List<string>()
            {
                packageData?["name"]?.Value<string>()
            });

            GitDependentiesLink playboxCurrentRepo = new GitDependentiesLink();
            playboxCurrentRepo.isHashedLock = true;
            playboxCurrentRepo.gitCommitHash = packageVersion[0].hash;
            playboxCurrentRepo.gitProjectName = "playbox-sdk";
            playboxCurrentRepo.gitOrganization = "playbox-technologies";
            playboxCurrentRepo.gitFilePath = "package.json";
            
            var playboxCurrentData = await GetPackageInfo(playboxCurrentRepo.GetRawGitRef());
            
            var packageCurrentData = JObject.Parse(playboxCurrentData);
            
            playbox_current_version = packageCurrentData?["version"]?.Value<string>();
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

        private async Task<string> GetPackageInfo(string url = "")
        {
            var res = await HttpHelper.GetAsync(
                url);
            
            return res.Body;
        }
    }
}

#endif