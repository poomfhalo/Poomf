using StealthGame;
using UnityEditor;
using UnityEngine;

namespace GW_Lib.Utility
{
    [CustomPropertyDrawer(typeof(MultiSelectType),true)]
    public class MultiSelectTypesDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            const string selectionsName = "selections";
            SerializedProperty selectionsProp = property.FindPropertyRelative(selectionsName);
            Rect namePos = position;
            float halfWidth = namePos.width / 2;
            namePos.width = halfWidth;

            Rect maskPos = EditorExtentions.MoveRectRight(halfWidth,namePos);
            EditorGUI.LabelField(namePos, property.displayName);

            Object target = property.serializedObject.targetObject;
            System.Object val = fieldInfo.GetValue(target);
            MultiSelectType actualObj = (MultiSelectType)val;

            selectionsProp.intValue = EditorGUI.MaskField(maskPos, selectionsProp.intValue, actualObj.possiblities);
        }
    }
}