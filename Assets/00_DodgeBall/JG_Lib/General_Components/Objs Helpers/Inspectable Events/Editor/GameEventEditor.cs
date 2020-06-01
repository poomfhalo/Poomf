using UnityEditor;
using UnityEngine;

namespace GW_Lib.Utility
{
    [CustomEditor(typeof(GameEvent))]
    public class GameEventEditor : Editor
    {
        GameEvent mySelf;
        private void OnEnable()
        {
            mySelf = target as GameEvent;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUI.enabled = EditorApplication.isPlaying;

            if (GUILayout.Button("Raise Event"))
            {
                mySelf.Raise();
            }
        }
    }
}