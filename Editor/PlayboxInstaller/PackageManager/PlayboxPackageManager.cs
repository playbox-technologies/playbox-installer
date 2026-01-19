#if UNITY_EDITOR

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

        private string _playboxOrganization = "playbox-technologies";

        private string publicPackages = "";
        
        private List<string> playboxBranches = new();
        
        
        [MenuItem("Playbox/Installer/Package Manager")]
        private static void UnpackArhivesMenu()
        {
            GetWindow<PlayboxPackageManager>("Playbox Package Manager");
        }

        private async void UpdateNewVersions()
        {
            GitDependentiesLink playboxAcrualRepo = new GitDependentiesLink();
            playboxAcrualRepo.isHashedLock = false;
            playboxAcrualRepo.gitBranch = "main";
            playboxAcrualRepo.gitProjectName = "playbox-sdk";
            playboxAcrualRepo.gitOrganization = "playbox-technologies";
            playboxAcrualRepo.gitFilePath = "package.json";

            Debug.Log(playboxAcrualRepo.GetBranchesURL());
            
            var branches = await HttpGET(playboxAcrualRepo.GetBranchesURL());
            
            var branchesJson = JArray.Parse(branches);
            
            foreach (var item in branchesJson)
            {
                playboxBranches.Add(item?["name"]?.Value<string>());
            }
            
            var playboxString =  await HttpGET(playboxAcrualRepo.GetRawGitRef());
            
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
            
            var playboxCurrentData = await HttpGET(playboxCurrentRepo.GetRawGitRef());
            
            var packageCurrentData = JObject.Parse(playboxCurrentData);
            
            playbox_current_version = packageCurrentData?["version"]?.Value<string>();
        }

        private void OnGUI()
        {
            PlayboxLayout.HorizontalLayout(() =>
            {
                if (GUILayout.Button("Update New Versions"))
                {
                    UpdateNewVersions();
                    
                    Debug.Log("Load Data");
                }
            });
            
            PlayboxLayout.HorizontalLayout(() =>
            {
                GUILayout.Label("Install Playbox");
                
                GUILayout.Label("Actual version");
                GUILayout.Label(playbox_actual_version);
                
                GUILayout.Label("Current version");
                GUILayout.Label(playbox_current_version);

                EditorGUILayout.Popup("", 0,playboxBranches.ToArray());
                
                if (GUILayout.Button("Install"))
                {
                    Debug.Log("Installing Playbox");
                }
            });

            PlayboxLayout.HorizontalLayout(() =>
            {
                GUILayout.Space(20);
                GUILayout.Label(publicPackages);    
            });
        }

        private static async Task<string> HttpGET(string url = "")
        {
            var res = await HttpHelper.GetAsync(
                url);
            
            return res.Body;
        }
    }
}

#endif