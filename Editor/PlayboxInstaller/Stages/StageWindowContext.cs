using UnityEditor;

#if UNITY_EDITOR

namespace Editor.PlayboxInstaller.Stages
{
    public class StageWindowContext
    {
        protected EditorWindow editorWindow;
        protected bool isEnableNextStage = false;
        
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