using UnityEditor;

#if UNITY_EDITOR

namespace PlayboxInstaller
{
    public class StageWindowContext
    {
        protected EditorWindow editorWindow;
        protected bool isEnableNextStage = false;
        
        protected string _nextStageButtonName = "Next Stage";

        public string nextStageButtonName
        {
            get => _nextStageButtonName;
            set => _nextStageButtonName = value;
        }

        public virtual void Initialize(EditorWindow window)
        {
            editorWindow = window;
        }
        
        public virtual void OnGUI()
        {
        }

        public bool IsEnableNextStage() => isEnableNextStage;
    }
}

#endif