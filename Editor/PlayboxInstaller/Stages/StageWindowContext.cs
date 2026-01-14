#if UNITY_EDITOR

using UnityEditor;

namespace PlayboxInstaller
{
    public class StageWindowContext
    {
        protected EditorWindow editorWindow;
        protected bool isEnableNextStage = false;

        protected string _nextStageButtonName = "Next Stage";
        protected string _stageName = "Stage";

        public string nextStageButtonName
        {
            get => _nextStageButtonName;
            set => _nextStageButtonName = value;
        }

        public virtual void Initialize(EditorWindow window, string stageName)
        {
            editorWindow = window;
            _stageName = stageName;
        }

        public virtual void OnGUI()
        {
        }

        public bool IsEnableNextStage() => isEnableNextStage;

        public void EnableNextStage()
        {
            EditorPrefs.SetBool(_stageName, isEnableNextStage = true);
        }

        public void LoadNextStage()
        {
            if(EditorPrefs.HasKey(_stageName))
                isEnableNextStage = EditorPrefs.GetBool(_stageName);
        }
    }
}

#endif