#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlayboxInstaller
{
    public class PlayboxInstallerWindow : EditorWindow
    {
        private readonly List<StageWindowContext> m_StageWindowContexts = new();
        private int m_SelectedStageIndex = 0;

        [MenuItem("PlayboxInstaller/Open Installer Window")]
        private static void UnpackArhivesMenu()
        {
            GetWindow<PlayboxInstallerWindow>("Playbox Installer Window");
        }

        private StageWindowContext CurrentStage()
        {
            return m_StageWindowContexts[Mathf.Clamp(m_SelectedStageIndex, 0, m_StageWindowContexts.Count - 1)];
        }

        private void CreateGUI()
        {
            m_StageWindowContexts.Clear();

            m_StageWindowContexts.Add(new InstallDependentiesStage());
            m_StageWindowContexts.Add(new DownloadPackagesStage());
            m_StageWindowContexts.Add(new InstallFirebaseStage());
            m_StageWindowContexts.Add(new InstallPlayboxStage());

            CurrentStage().Initialize(this);
        }

        private void OnGUI()
        {
            var stage = CurrentStage();

            stage.OnGUI();

            GUI.enabled = stage.IsEnableNextStage();

            if (!(m_SelectedStageIndex >= m_StageWindowContexts.Count - 1))
                if (GUILayout.Button(stage.nextStageButtonName))
                {
                    m_SelectedStageIndex++;
                    m_SelectedStageIndex = Mathf.Clamp(m_SelectedStageIndex, 0, m_StageWindowContexts.Count - 1);

                    stage = CurrentStage();

                    stage.Initialize(this);
                }

            GUI.enabled = true;
        }
    }
}

#endif