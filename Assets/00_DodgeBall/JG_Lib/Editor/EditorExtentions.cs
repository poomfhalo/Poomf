using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace GW_Lib
{
    public delegate void FunctionToWrap();
    public static class SerilizedPropsExtention
    {
        public static void SwapPositionsInArray(this SerializedProperty prop,int startIndex, int endIndex)
        {
            if (prop.isArray == false)
            {
                throw new UnityException("this is not an array " + prop.name);
            }
            if (endIndex < 0 || endIndex >= prop.arraySize)
            {
                return;
            }
            if (startIndex<0||endIndex>=prop.arraySize)
            {
                return;
            }

            prop.serializedObject.Update();
            SerializedProperty startProp = prop.GetArrayElementAtIndex(startIndex);
            SerializedProperty endProp = prop.GetArrayElementAtIndex(endIndex);

            startProp.serializedObject.Update();
            endProp.serializedObject.Update();

            UnityEngine.Object startObj = startProp.objectReferenceValue;
            UnityEngine.Object endObj = endProp.objectReferenceValue;
            startProp.objectReferenceValue = endObj;
            endProp.objectReferenceValue = startObj;

            startProp.serializedObject.ApplyModifiedProperties();
            endProp.serializedObject.ApplyModifiedProperties();
            prop.serializedObject.ApplyModifiedProperties();

        }
        public static void AddObjToArrayAt<T>(this SerializedProperty prop,T ObjToAddAt,int to)
            where T:UnityEngine.Object
        {
            prop.serializedObject.Update();
            if (prop.isArray == false)
            {
                throw new UnityException("this is not an array " + prop.name);
            }
            if (ObjToAddAt == null)
            {
                throw new UnityException("The Object We're trying to add is null");
            }
            if (to<0||to>=prop.arraySize)
            {
                throw new UnityException("the spot we're trying to add the object into the array is out of bounds");
            }
            prop.InsertArrayElementAtIndex(to);
            SerializedProperty addedPropSlot = prop.GetArrayElementAtIndex(to);
            addedPropSlot.objectReferenceValue = ObjToAddAt;
            prop.serializedObject.ApplyModifiedProperties();
        }
        public static int FindIndexOf<T>(this SerializedProperty prop, T objToFindIndexOf) where T:UnityEngine.Object
        {
            prop.serializedObject.Update();
            int index = 0;
            if (objToFindIndexOf == null)
            {
                throw new UnityException("the object we're trying to find its index is null");
            }
            if (prop.isArray == false)
            {
                throw new UnityException("this is not an array " + prop.name);
            }
            for (int i = 0; i < prop.arraySize; i++)
            {
                SerializedProperty tempProp = prop.GetArrayElementAtIndex(i);
                if (tempProp.objectReferenceInstanceIDValue==objToFindIndexOf.GetInstanceID()
                    ||tempProp.objectReferenceValue==objToFindIndexOf)
                {
                    index = i;
                    break;
                }
            }
            prop.serializedObject.ApplyModifiedProperties();
            return index;
        }
        public static void AddObjToArray<T>(this SerializedProperty prop,T objToAdd) where T:UnityEngine. Object
        {
            if (objToAdd == null)
            {
                throw new UnityException("the object to add is online" + objToAdd.name);
            }
            if (prop.isArray == false)
            {
                throw new UnityException("this is not an array " + prop.name);
            }
            prop.serializedObject.Update();

            prop.InsertArrayElementAtIndex(prop.arraySize);
            prop.GetArrayElementAtIndex(prop.arraySize - 1).objectReferenceValue = objToAdd;

            prop.serializedObject.ApplyModifiedProperties();
        }
        public static void RemoveObjFromArrayAt(this SerializedProperty prop,int index)
        {
            if (prop.isArray == false)
            {
                throw new UnityException("this is not an array " + prop.name);
            }
            if (index>prop.arraySize-1)
            {
                throw new UnityException("array size is " + prop.arraySize + "while you are trying to remove index of " + index);
            }
            if (prop.arraySize<=0)
            {
                throw new UnityException("this array is empty brah " + prop.name);
            }
            prop.serializedObject.Update();
            if (prop.GetArrayElementAtIndex(index).objectReferenceValue)
            {
                prop.DeleteArrayElementAtIndex(index);
            }
            prop.DeleteArrayElementAtIndex(index);
            prop.serializedObject.ApplyModifiedProperties();
        }
        public static void RemoveObjFromArray<T>(this SerializedProperty prop, T objToRemove) where T :UnityEngine.Object
        {
            if (prop == null)
            {
                throw new UnityException("Can Not Find Parent");
            }
            if (prop.isArray == false)
            {
                throw new UnityException("this is not an array " + prop.name);
            }
            if (objToRemove == null)
            {
                throw new UnityException("element to remove is null");
            }
            prop.serializedObject.Update();
            for (int i = 0; i < prop.arraySize; i++)
            {
                SerializedProperty elementProp = prop.GetArrayElementAtIndex(i);

                if (elementProp.objectReferenceInstanceIDValue == objToRemove.GetInstanceID()
                    || elementProp.objectReferenceValue == objToRemove)
                {
                    RemoveObjFromArrayAt(prop, i);
                    return;
                }
            }
            throw new UnityException("couldnt find " + objToRemove.name+ 
                " in the proprety(which shuld be an array) " + prop.name);
        }
    }
    public static class EditorExtentions
    {
        public static string[] GetAxesNames()
        {
            var inputManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];

            SerializedObject obj = new SerializedObject(inputManager);

            SerializedProperty axisArray = obj.FindProperty("m_Axes");

            if (axisArray.arraySize == 0)
                Debug.Log("No Axes");
            string[] axesNames = new string[axisArray.arraySize];
            for (int i = 0; i < axisArray.arraySize; ++i)
            {
                var axis = axisArray.GetArrayElementAtIndex(i);
                var name = axis.FindPropertyRelative("m_Name").stringValue;
                axesNames[i] = name;
            }
            return axesNames;
        }
        public static void FillArray(SerializedProperty inProp,UnityEngine.Object[] byObjs)
        {
            inProp.ClearArray();
            inProp.arraySize = byObjs.Length;
            for (int i=0; i<inProp.arraySize ;i++)
            {
                inProp.GetArrayElementAtIndex(i).objectReferenceValue=byObjs[i];
            }
        }
        public static T[] CollectFilesInProject<T>(string filter) where T:UnityEngine.Object
        {
            string[] itemsGuids = AssetDatabase.FindAssets(filter);
            List<T> items = new List<T>();
            foreach (string guid in itemsGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T item = AssetDatabase.LoadAssetAtPath<T>(path);
                items.Add(item);
            }
            return items.ToArray();
        }

        public static Rect MoveRectRight(float space, Rect rectToMove)
        {
            rectToMove.position = new Vector2(rectToMove.position.x + space, rectToMove.position.y);
            return rectToMove;
        }

        public static Rect MoveRectDown(float space, Rect rectToMove)
        {
            rectToMove.position = new Vector2(rectToMove.position.x, rectToMove.position.y + space);
            return rectToMove;
        }
        public static int SelectableFromArray(int currentSelection,Rect atPos,SerializedProperty arrayProp,string label)
        {
            Dictionary<string, int> nameToIndexTable;
            string[] names = GetNamesOfObjectsInArray(arrayProp, out nameToIndexTable);

            int displayedNameIndex = currentSelection;
            displayedNameIndex = EditorGUI.Popup(atPos, label, displayedNameIndex, names);
            bool nameOutOfRange = displayedNameIndex < 0 || displayedNameIndex >= names.Length;

            int selectedIndexOfArray = 0;
            if (nameOutOfRange == false)
            {
                string displayedName = names[displayedNameIndex];
                selectedIndexOfArray = nameToIndexTable[displayedName];
            }
            return selectedIndexOfArray;
        }
        public static string[] GetNamesOfObjectsInArray(SerializedProperty arrayProp,out Dictionary<string,int> nameToIndexTable)
        {
            List<string> names = new List<string>();
            nameToIndexTable = new Dictionary<string, int>();
            if (arrayProp.isArray==false)
            {
                Debug.LogError("Property is not an array, returning empty names array");
                return names.ToArray();
            }
            for (int i = 0; i < arrayProp.arraySize; i++)
            {
                UnityEngine.Object obj = arrayProp.GetArrayElementAtIndex(i).objectReferenceValue;
                if (obj!=null)
                {
                    nameToIndexTable[obj.name] = i;
                    names.Add(obj.name);
                }
            }
            return names.ToArray();
        }
        /// <summary>
        /// Draws an Array Property, at a position, and returns the last position in the array
        /// </summary>
        /// <returns></returns>
        public static Rect DrawArray(SerializedProperty arrayProp, Rect atPos, float singleLineHeigth)
        {
            Rect arrayPos = atPos;
            arrayPos.height = singleLineHeigth;

            EditorGUI.PropertyField(arrayPos, arrayProp);
            Rect lastElementPos = arrayPos;
            if (arrayProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                lastElementPos = MoveRectDown(singleLineHeigth, lastElementPos);
                arrayProp.arraySize = EditorGUI.IntField(lastElementPos, arrayProp.arraySize);

                for (int i = 0; i < arrayProp.arraySize; i++)
                {
                    lastElementPos = MoveRectDown(singleLineHeigth, lastElementPos);
                    SerializedProperty element = arrayProp.GetArrayElementAtIndex(i);
                    EditorGUI.PropertyField(lastElementPos, element);
                }
                EditorGUI.indentLevel--;
            }
            return lastElementPos;
        }

        public static void DisplayMonoScript<T>(T mono)where T:MonoBehaviour
        {
            MonoScript script = MonoScript.FromMonoBehaviour(mono as MonoBehaviour);
            EditorGUILayout.ObjectField("Script " ,script, typeof(MonoScript), false);
        }
        public static void DisplayScriptableScript<T>(T scriptableObj) where T : ScriptableObject
        {
            MonoScript script = MonoScript.FromScriptableObject(scriptableObj as ScriptableObject);
            EditorGUILayout.ObjectField("Script ", script, typeof(MonoScript), false);
        }
        public static void FindAllNamesNonAbsChildren(Type t, out string[] allNames, out Type[] AllTypes)
        {
            Type[] allTypes = t.Assembly.GetTypes();
            List<Type> allTypesList = new List<Type>();

            for (int i = 0; i < allTypes.Length; i++)
            {
                if (allTypes[i].IsAbstract == false && allTypes[i].IsSubclassOf(t) == true)
                {
                    allTypesList.Add(allTypes[i]);
                }
            }
            AllTypes = allTypesList.ToArray();
            allNames = new string[allTypesList.Count];
            for (int i = 0; i < allNames.Length; i++)
            {
                allNames[i] = allTypesList[i].Name;
            }
        }
        public static float GetCurrViewWidth(float dividedBy)
        {
            return EditorGUIUtility.currentViewWidth / dividedBy;
        }
        public static GUIStyle ChangeStyle(int size,bool boldNess)
        {
            GUIStyle myStyle = new GUIStyle(GUI.skin.label);
            if (boldNess==true)
            {
                myStyle.font = EditorStyles.boldFont;
            }
            myStyle.fontSize = size;
            return myStyle;
        }
        public static void WrapInVertical(GUIStyle styleOfWrap,int indentation, FunctionToWrap FW)
        {
            if (styleOfWrap == null)
            {
                EditorGUILayout.BeginVertical();
            }
            else
            {
                EditorGUILayout.BeginVertical(styleOfWrap);
            }
            EditorGUI.indentLevel += indentation;
            FW();
            EditorGUI.indentLevel -= indentation;
            EditorGUILayout.EndVertical();
        }
        public static void WrapInHori(GUIStyle styleOfWrap, int indentation, FunctionToWrap FW)
        {
            if (styleOfWrap == null)
            {
                EditorGUILayout.BeginHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal(styleOfWrap);
            }
            
            EditorGUI.indentLevel += indentation;
            FW();
            EditorGUI.indentLevel -= indentation;
            EditorGUILayout.EndHorizontal();
        }

        public static void FillArray(SerializedObject so, SerializedProperty arrayProp, UnityEngine.Object[] collectedHeads)
        {
            so.Update();

            arrayProp.ClearArray();
            arrayProp.arraySize = collectedHeads.Length;

            for (int i = 0; i < arrayProp.arraySize; i++)
            {
                arrayProp.GetArrayElementAtIndex(i).objectReferenceValue = collectedHeads[i];
            }

            so.ApplyModifiedProperties();
        }
    }
}