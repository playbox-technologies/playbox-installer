#if UNITY_EDITOR

using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PlayboxInstaller;
using UnityEditor;
using UnityEngine;

namespace Editor.PlayboxInstaller.PackageManager
{
    public class PlayboxPackageManager : EditorWindow
    {
        private string playbox_package_link =
            "https://raw.githubusercontent.com/playbox-technologies/playbox-sdk/refs/heads/main/package.json";

        private string playbox_actual_version = "0.0.1";
        private string playbox_current_version = "0.0.1";
        
        
        [MenuItem("Playbox/Installer/Package Manager")]
        private static void UnpackArhivesMenu()
        {
            GetWindow<PlayboxPackageManager>("Playbox Package Manager");
        }

        private async void CreateGUI()
        {
            var playboxVersion =  await GetActualPlayboxVersion();
            
            Debug.Log(await GetActualPlayboxVersion("44fd2a032091a1931bcc3c8daff60ab039bf3fe7"));
            
            playbox_actual_version = playboxVersion;

            var packageVersion = LockedRepositoryHelper.GetDependencyVersion("playbox");

            playbox_current_version = await GetActualPlayboxVersion(packageVersion.hash);
        }

        private void OnGUI()
        {
            PlayboxLayout.HorizontalLayout(() =>
            {
                GUILayout.Label("Install Playbox");
                
                GUILayout.Label("Actual version");
                GUILayout.Label(playbox_actual_version);
                GUILayout.Label(playbox_current_version);
                
                if (GUILayout.Button("Install"))
                {
                    Debug.Log("Installing Playbox");
                }
            });
        }

        private async Task<string> GetActualPlayboxVersion(string target = "refs/heads/main")
        {
            var res = await HttpHelper.GetAsync(
                $"https://raw.githubusercontent.com/playbox-technologies/playbox-sdk/{target}/package.json");

            var packageJson = JObject.Parse(res.Body);
            
            return packageJson["version"]?.ToString();
        }
    }
}

#endif