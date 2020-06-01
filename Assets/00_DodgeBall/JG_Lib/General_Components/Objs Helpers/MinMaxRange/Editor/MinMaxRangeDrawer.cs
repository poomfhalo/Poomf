using UnityEngine;
using UnityEditor;
namespace GW_Lib.Utility
{
    [CustomPropertyDrawer(typeof(MinMaxRange))]
    public class MinMaxRangeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 3;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            GUI.Box(position,GUIContent.none,GUI.skin.box);
            SerializedProperty minProp = property.FindPropertyRelative("rMin");
            SerializedProperty maxProp = property.FindPropertyRelative("rMax");

            SerializedProperty absMinProp = property.FindPropertyRelative("absMin");
            SerializedProperty absMaxProp = property.FindPropertyRelative("absMax");
            //Get Initial Values
            float min = minProp.floatValue;
            float max = maxProp.floatValue;
            float absMin = absMinProp.floatValue;
            float absMax = absMaxProp.floatValue;

            float solePropHegith = position.height/3.0f;
            float xDamp = 50;
            
            //Value Setters
            Rect absMinPos = new Rect(position.x, position.y+2*solePropHegith, xDamp, solePropHegith);
            absMin = EditorGUI.FloatField(absMinPos, absMin);
            Rect sliderPos = new Rect(position.x+xDamp, position.y+2*solePropHegith, position.width-2*xDamp, solePropHegith);
            EditorGUI.MinMaxSlider(sliderPos, ref min, ref max, absMin, absMax);
            Rect absMaxPos = new Rect(position.x + position.width - xDamp, position.y+2*solePropHegith, xDamp, solePropHegith);
            absMax = EditorGUI.FloatField(absMaxPos, absMax);
            
            //Displayers
            Rect fromPos = new Rect(position.x+position.width/5.0f, position.y+solePropHegith, position.width / 4.0f, solePropHegith);
            Rect toPos = new Rect(position.x + position.width / 1.7f, position.y+solePropHegith, position.width / 4.0f, solePropHegith);

            min = EditorGUI.FloatField(fromPos,  min);
            max = EditorGUI.FloatField(toPos,  max);

            minProp.floatValue = min;
            maxProp.floatValue = max;
            absMinProp.floatValue = absMin;
            absMaxProp.floatValue = absMax;

            Rect titlePos = new Rect(position.x, position.y, position.width, solePropHegith);
            string title = "Range: " + property.name;
            EditorGUI.LabelField(titlePos, title,EditorStyles.boldLabel);

            EditorGUI.EndProperty();
        }
    }
}