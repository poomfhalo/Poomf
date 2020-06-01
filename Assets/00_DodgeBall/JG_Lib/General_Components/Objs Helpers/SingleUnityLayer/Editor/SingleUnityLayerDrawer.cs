using UnityEditor;
using UnityEngine;

namespace GW_Lib.Utility
{
    [CustomPropertyDrawer(typeof(SingleUnityLayer))]
    public class SingleUnityLayerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty layerProp = property.FindPropertyRelative("layerIndex");
            layerProp.intValue = EditorGUI.LayerField(position,property.displayName,layerProp.intValue);
        }
    }
}