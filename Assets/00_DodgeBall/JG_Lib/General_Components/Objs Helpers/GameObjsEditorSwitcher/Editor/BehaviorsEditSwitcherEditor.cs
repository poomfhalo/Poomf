using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace GW_Lib.Utility
{
    [CustomEditor(typeof(BehaviorsEditSwitcher))]
    public class BehaviorsEditSwitcherEditor : Editor
    {
        BehaviorsEditSwitcher mySelf = null;
        private void OnEnable()
        {
            mySelf = target as BehaviorsEditSwitcher;
            if(mySelf == null || serializedObject == null)
            {
                DestroyImmediate(this);
                return;
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if(GUILayout.Button("GetBehaviors"))
            {
                mySelf.GetBehaviors();
                SetDirt();
            }
        }
        void OnSceneGUI()
        {
            Event e = Event.current;

            if(e.isKey == false)
            {
                return;
            }

            int x = -1;
            if(e.keyCode == KeyCode.Alpha1)
            {
                x = 0;
            }
            if(e.keyCode == KeyCode.Alpha2)
            {
                x = 1;
            }
            if(e.keyCode == KeyCode.Alpha3)
            {
                x = 2;
            }
            if(e.keyCode == KeyCode.Alpha4)
            {
                x = 3;
            }
            if(e.keyCode == KeyCode.Alpha5)
            {
                x = 4;
            }
            if(e.keyCode == KeyCode.Alpha6)
            {
                x = 5;
            }
            if(e.keyCode == KeyCode.Alpha7)
            {
                x = 6;
            }
            if(e.keyCode == KeyCode.Alpha8)
            {
                x = 7;
            }
            if(e.keyCode == KeyCode.Alpha9)
            {
                x = 8;
            }
            if(e.keyCode == KeyCode.Alpha0)
            {
                x = 9;
            }

            if(x==-1)
            {
                return;
            }

            e.Use();
            mySelf.ActivateBehavior(x);
            SetDirt();
        }
        private void SetDirt()
        {
            EditorUtility.SetDirty(mySelf);

            if(mySelf.gameObject.scene.path =="")
            {
                return;
            }

            EditorSceneManager.MarkSceneDirty(mySelf.gameObject.scene);
        }
    }
}