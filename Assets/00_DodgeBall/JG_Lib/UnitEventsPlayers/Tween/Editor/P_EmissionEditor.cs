using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace GW_Lib.Utility
{
    [CustomEditor(typeof(P_Emission))]
    public class P_EmissionEditor : Editor
    {
        P_Emission mySelf;
        void OnEnable()
        {
            mySelf = target as P_Emission;
            if(mySelf == null||serializedObject == null)
            {
                DestroyImmediate(this);
                return;
            }
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUIStyle style = EditorExtentions.ChangeStyle(13, true);
            EditorGUILayout.LabelField("Total Time : " + GetTotalTime(),style);
        }

        private float GetTotalTime()
        {
            float f = 0;
            foreach (var action in mySelf.Actions)
            {
                f = f + action.loops*action.time;
            }
            f = f + mySelf.RestTime;
            return f;
        }
    }
}