using UnityEditor;
using UnityEngine;

namespace GW_Lib.Utility
{
    [CustomEditor(typeof(UnitEventsCollection))]
    public class UnitEventsCollectionEditor : Editor
    {
        UnitEventsCollection mySelf;
        void OnEnable()
        {
            mySelf = target as UnitEventsCollection;
            if (mySelf == null || serializedObject == null)
            {
                DestroyImmediate(this);
                return;
            }
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (!Application.isPlaying && GUILayout.Button("Refresh IDS By Order"))
            {
                UnitPlayable[] playables = mySelf.GetComponents<UnitPlayable>();
                for (int i = 0; i < playables.Length; i++)
                {
                    if (i == playables[i].ID)
                    {
                        continue;
                    }
                    playables[i].SetID(i);
                    EditorUtility.SetDirty(mySelf);
                    EditorUtility.SetDirty(mySelf.gameObject);
                }
            }
        }

    }
}