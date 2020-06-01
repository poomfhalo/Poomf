using UnityEditor;
using UnityEngine;

namespace GW_Lib.Utility
{
    public abstract class GenericGameEventEditor<ParameterType> : Editor
    {
        GenericGameEvent<ParameterType> mySelf;
        private void OnEnable()
        {
            mySelf = target as GenericGameEvent<ParameterType>;
            if (mySelf==null||serializedObject==null)
            {
                DestroyImmediate(this);
                return;
            }
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUI.enabled = Application.isPlaying;

            if (GUILayout.Button("Raise Event"))
            {
                mySelf.Raise(mySelf.SampleParameter);
            }
        }
    }
}