using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GW_Lib.Utility
{
    [CustomEditor(typeof(PoolsManager))]
    public class PoolsManagerEditor : Editor
    {
        PoolsManager mySelf;
        private void OnEnable()
        {
            mySelf = target as PoolsManager;
            if (mySelf == null || serializedObject == null)
            {
                DestroyImmediate(this);
                return;
            }
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (Application.isPlaying)
            {
                return;
            }
            EditorGUILayout.BeginHorizontal();
            bool setUUIDs = GUILayout.Button("Set UUID for PoolablePrefabs");
            bool getAll = GUILayout.Button("Collect Poolables");
            EditorGUILayout.EndHorizontal();
            if (setUUIDs)
            {
                string[] prefabsGUIDS = AssetDatabase.FindAssets("t:prefab", new string[] { mySelf.prefabsPath });
                foreach (string s in prefabsGUIDS)
                {
                    string path = AssetDatabase.GUIDToAssetPath(s);
                    GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                    Poolable p = prefab.GetComponent<Poolable>();
                    if (p == null) { continue; }
                    int hash = Animator.StringToHash(s);
                    if (p.prefabID == hash) { continue; }

                    p.SetPrefabID(hash);
                    EditorUtility.SetDirty(prefab);
                    EditorUtility.SetDirty(p);
                }
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            if (getAll)
            {
                string[] prefabsGUIDS = AssetDatabase.FindAssets("t:prefab", new string[] { mySelf.prefabsPath });
                foreach (string s in prefabsGUIDS)
                {
                    string path = AssetDatabase.GUIDToAssetPath(s);
                    GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                    Poolable p = prefab.GetComponent<Poolable>();
                    if (p == null) { continue; }
                    if (mySelf.TryAddToStartingPools(p) == false) { continue; }
                    EditorUtility.SetDirty(mySelf);
                    EditorUtility.SetDirty(mySelf.gameObject);
                }
            }

        }
    }
}