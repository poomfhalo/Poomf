using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace EPOOutline
{
    [CustomEditor(typeof(Outliner))]
    public class OutlinerEditor : Editor
    {
        [SerializeField] private bool isIterationsOpen = false;

        [SerializeField] private bool isShiftsOpen = false;

        [SerializeField] private bool isResolutionSettingsOpen = false;

        public override void OnInspectorGUI()
        {
#if SRP_OUTLINE
            if (PipelineAssetUtility.IsURPOrLWRP(PipelineAssetUtility.CurrentAsset))
            {
                EditorGUILayout.HelpBox(
                    "This component is doing nothing in URP/LWRP pipeline. Use render feature settings on the URP/LWRP renderer. You can safely remove it in URP/LWRP setup.",
                    MessageType.Info);
                return;
            }
#endif

            var blurIterations = serializedObject.FindProperty("blurIterations");
            var dilateIterations = serializedObject.FindProperty("dilateIterations");
            var primaryRendererScale = serializedObject.FindProperty("primaryRendererScale");
            var infoBufferScale = serializedObject.FindProperty("infoBufferScale");
            var blurShift = serializedObject.FindProperty("blurShift");
            var dilateShift = serializedObject.FindProperty("dilateShift");
            var usingInfoBuffer = serializedObject.FindProperty("usingInfoBuffer");

            isIterationsOpen = EditorGUILayout.Foldout(isIterationsOpen, "Iterations");

            if (isIterationsOpen)
            {
                EditorGUI.indentLevel = 1;

                EditorGUILayout.HelpBox("It's not recommended to set high values because it's performance heavy operation",
                    MessageType.Warning);
                EditorGUILayout.PropertyField(blurIterations, new GUIContent(blurIterations.displayName,
                    "Higher value is the way to get more blurry outline. It requires a lot of resources to handle"));

                EditorGUILayout.HelpBox("It's not recommended to set high values because it's performance heavy operation",
                    MessageType.Warning);
                EditorGUILayout.PropertyField(dilateIterations, new GUIContent(dilateIterations.displayName,
                    "Higher value is the way to get more thick outline. More or less heavy on resources. Don't set to high values"));

                EditorGUI.indentLevel = 0;
            }

            isShiftsOpen = EditorGUILayout.Foldout(isShiftsOpen, "Global settings");
            if (isShiftsOpen)
            {
                EditorGUI.indentLevel = 1;

                EditorGUILayout.PropertyField(blurShift, new GUIContent(blurShift.displayName, "How much blur will be scaled"));
                EditorGUILayout.PropertyField(dilateShift, new GUIContent(dilateShift.displayName, "How much dilate will be scaled"));

                EditorGUI.indentLevel = 0;
            }

            isResolutionSettingsOpen = EditorGUILayout.Foldout(isResolutionSettingsOpen, "Buffer settings");

            if (isResolutionSettingsOpen)
            {
                EditorGUI.indentLevel = 1;

                EditorGUILayout.PropertyField(usingInfoBuffer, new GUIContent(usingInfoBuffer.displayName,
                    "Allows to use different scaling settings for each outlinable. Requires some resources to handle."));

                EditorGUILayout.HelpBox("The lower value is set, the best performance you get", MessageType.Info);
                EditorGUILayout.PropertyField(primaryRendererScale, new GUIContent(primaryRendererScale.displayName,
                    "Defines scale of the effect buffer. The lower the better performance you get."));

                if (usingInfoBuffer.boolValue)
                {
                    EditorGUILayout.HelpBox("The lower value is set, the best performance you get", MessageType.Info);
                    EditorGUILayout.PropertyField(infoBufferScale, new GUIContent(infoBufferScale.displayName,
                        "Defines scale of the info buffer. The lower the better performance you get."));
                }

                EditorGUI.indentLevel = 0;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}