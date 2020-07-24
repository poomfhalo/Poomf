using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MovingObjShooter))]
public class MovingObjShooterEditor : Editor
{
    MovingObjShooter mySelf = null;
    void OnEnable()
    {
        mySelf = target as MovingObjShooter;
        if(mySelf == null || serializedObject == null)
        {
            DestroyImmediate(this);
            return;
        }
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if(GUILayout.Button("Shoot"))
        {
            mySelf.Shoot();
        }
    }
}
