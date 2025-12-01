using System.Collections.Generic;
using PlayboxInstaller;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public class InstallFacebookStage : StageWindowContext
{
    private List<UnityPackageData> unityPackages = new();

    public override void Initialize(EditorWindow window, string stageName)
    {
        base.Initialize(window, stageName);
        unityPackages = FacebookArhivesData.UnpackArhives();
    }


    public override void OnGUI()
    {
        base.OnGUI();

        GUILayout.Space(20);

        GUILayout.BeginVertical();

        GUILayout.Label("Install Facebook packages");

        GUILayout.Space(10);

        foreach (var item in unityPackages)
        {
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();

            GUILayout.Space(10);

            GUILayout.Label(item.packageName);

            item.isImporting = GUILayout.Toggle(item.isImporting, "Importing", GUILayout.ExpandWidth(false));

            GUILayout.Space(10);
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Install Arhives"))
        {
            SaveUnityPackagesState();

            FacebookArhivesData.InstallArhives(unityPackages);

            EnableNextStage();

            LoadUnityPackagesState();

            LoadNextStage();
        }

        GUILayout.EndVertical();
    }

    private void SaveUnityPackagesState()
    {
        foreach (var item in unityPackages)
        {
            EditorPrefs.SetBool(item.packageName, item.isImporting);
        }
    }

    private void LoadUnityPackagesState()
    {
        foreach (var item in unityPackages)
        {
            if (EditorPrefs.HasKey(item.packageName))
                item.isImporting = EditorPrefs.GetBool(item.packageName);
        }
    }
}

#endif