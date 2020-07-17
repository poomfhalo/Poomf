using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GW_Lib;
using System.Linq;

[CustomEditor(typeof(PersistantSOHolder))]
public class PersistantSOHolderEditor : Editor
{
    PersistantSOHolder mySelf;
    void OnEnable()
    {
        mySelf = target as PersistantSOHolder;
        if (mySelf == null || serializedObject == null)
        {
            DestroyImmediate(this);
            return;
        }
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Scan For Persistant SOs"))
        {
            List<PersistantSO> sos = EditorExtentions.CollectFilesInProject<PersistantSO>("t:PersistantSO").ToList();
            mySelf.SetObjs(sos);
            EditorUtility.SetDirty(mySelf);
            EditorUtility.SetDirty(mySelf.gameObject);
        }
    }
}