using UnityEditor;
using UnityEngine;

namespace GW_Lib.Utility
{
    [CustomEditor(typeof(ArcArrangement))]
    public class ArcArrangementEditor : Editor
    {
        private static readonly Vector3 UP = new Vector3(0,1,0);
        private ArcArrangement mySelf = null;
        private void OnEnable()
        {
            mySelf = target as ArcArrangement;
            if(mySelf == null || serializedObject == null)
            {
                DestroyImmediate(this);
                return;
            }
        }
        private void OnSceneGUI()
        {
            Handles.color = Color.cyan;
            Handles.DrawWireArc(mySelf.transform.position,UP,mySelf.transform.forward,mySelf.Angle,mySelf.Radious);
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (mySelf.MoveInEditor)
            {
                foreach (Transform t in mySelf.Arrangeables)
                {
                    EditorUtility.SetDirty(t);
                }
            }
        }
    }
}