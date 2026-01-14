#if UNITY_EDITOR

using System;
using UnityEditor;

namespace Editor.PlayboxInstaller.PackageManager
{
    public class PlayboxPackageManager : EditorWindow
    {
        [MenuItem("Playbox/Package Manager")]
        private static void UnpackArhivesMenu()
        {
            GetWindow<PlayboxPackageManager>("Playbox Package Manager");
        }
        
        private void OnGUI()
        {
            
        }
    }
}

#endif