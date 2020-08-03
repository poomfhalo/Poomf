using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharaSkinData))]
public class CharaSkinDataEditor : Editor
{
    CharaSkinData mySelf = null;

    void OnEnable()
    {
        mySelf = target as CharaSkinData;
        if(mySelf == null||serializedObject == null)
        {
            DestroyImmediate(this);
            return;
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if(GUILayout.Button("Print Plain Data"))
        {
            Debug.Log(new CharaSkinDataPlain(mySelf).ToString());
        }
        if (GUILayout.Button("Print Actual Data"))
        {
            Debug.Log(mySelf.ToString());
        }
    }
}
