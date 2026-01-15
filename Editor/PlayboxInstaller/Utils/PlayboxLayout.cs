using System;
using UnityEngine;

#if UNITY_EDITOR


namespace PlayboxInstaller
{
    public static class PlayboxLayout
    {
        public static void HorizontalLayout(Action body)
        {
            GUILayout.BeginHorizontal();
            
            body?.Invoke();
            
            GUILayout.EndHorizontal();
        }
        
        public static void VerticalLayout(Action body)
        {
            GUILayout.BeginVertical();
            
            body?.Invoke();
            
            GUILayout.EndVertical();
        }
    }
}

#endif