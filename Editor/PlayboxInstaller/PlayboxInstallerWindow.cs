#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Editor.PlayboxInstaller.Stages;

namespace Editor.PlayboxInstaller
{
    public class PlayboxInstallerWindow : EditorWindow
    {
        private readonly List<StageWindowContext> m_StageWindowContexts = new();
        private int m_SelectedStageIndex = 0;

        [MenuItem("PlayboxInstaller/Open Installer Window")]
        private static void UnpackArhivesMenu()
        {
            GetWindow<PlayboxInstallerWindow>("Arhives Window");
        }

        private StageWindowContext CurrentStage() => m_StageWindowContexts[m_SelectedStageIndex];

        private void CreateGUI()
        {
            m_StageWindowContexts.Add(new InstallDependentiesStage());
            m_StageWindowContexts.Add(new DownloadPackagesStage());
            m_StageWindowContexts.Add(new InstallFirebaseStage());

            CurrentStage().Initialize(this);
        }

        private void OnGUI()
        {
            var stage = CurrentStage();
            
            m_SelectedStageIndex = Mathf.Clamp(m_SelectedStageIndex, 0, m_StageWindowContexts.Count - 1);
            
            stage.OnGUI();

            GUI.enabled = stage.IsEnableNextStage();

            if (GUILayout.Button("Next Stage"))
            {
                m_SelectedStageIndex++;
                m_SelectedStageIndex = Mathf.Clamp(m_SelectedStageIndex, 0, m_StageWindowContexts.Count - 1);
                
                stage.Initialize(this);
            }
            
            GUI.enabled = true;
        }
    }
}

#endif
