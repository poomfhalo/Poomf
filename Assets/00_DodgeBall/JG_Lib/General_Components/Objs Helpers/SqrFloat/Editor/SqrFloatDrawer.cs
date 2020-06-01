using GW_Lib;
using UnityEditor;
using UnityEngine;

namespace GW_Lib.Utility
{
    [CustomPropertyDrawer(typeof(SqrFloat))]
    public class SqrFloatDrawer : PropertyDrawer
    {
        const string fName = "f";
        const string sqrFName = "sqrF";
        SerializedProperty f, sqrF;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float oneLine = EditorGUIUtility.singleLineHeight;
            return oneLine;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float thirdWidth = EditorExtentions.GetCurrViewWidth(3);

            Rect titlePos = position;
            titlePos.width = thirdWidth;
            EditorGUI.LabelField(titlePos, property.displayName);

            f = property.FindPropertyRelative(fName);
            sqrF = property.FindPropertyRelative(sqrFName);

            Rect fPos = EditorExtentions.MoveRectRight(thirdWidth, titlePos);
            f.floatValue = EditorGUI.FloatField(fPos, f.floatValue);
            Rect sqrFPos = EditorExtentions.MoveRectRight(thirdWidth, fPos);
            EditorGUI.LabelField(sqrFPos, sqrF.floatValue.ToString());

            CorrectSqruaredValue();

            EditorGUI.EndProperty();
        }

        private void CorrectSqruaredValue()
        {
            float correctSqrVal = f.floatValue * f.floatValue;
            if (correctSqrVal != sqrF.floatValue)
            {
                sqrF.floatValue = correctSqrVal;
            }
        }
    }
}
