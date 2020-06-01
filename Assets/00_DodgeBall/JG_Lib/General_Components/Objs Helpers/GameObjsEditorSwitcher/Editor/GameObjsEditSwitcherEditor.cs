using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
namespace GW_Lib.Utility
{
    [CustomEditor(typeof(GameObjsEditSwitcher))]
    public class GameObjsEditSwitcherEditor : Editor
    {
        GameObjsEditSwitcher mySelf;
        private void OnEnable()
        {
            mySelf = target as GameObjsEditSwitcher;
            if(mySelf == null||serializedObject == null)
            {
                DestroyImmediate(this);
                return;
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
            mySelf.ActivateObj(x);
            EditorUtility.SetDirty(mySelf);

            if(mySelf.gameObject.scene.path =="")
            {
                return;
            }

            EditorSceneManager.MarkSceneDirty(mySelf.gameObject.scene);
        }
    }
}