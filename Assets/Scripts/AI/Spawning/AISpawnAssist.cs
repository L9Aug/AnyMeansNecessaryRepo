using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AnyMeansNecessaryAI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class AISpawnAssist : MonoBehaviour
{

    public AITypes TypeOfAI;
    public List<MonoScript> ScriptsToSpawnWith = new List<MonoScript>();
}

#if UNITY_EDITOR
[CustomEditor(typeof(AISpawnAssist))]
public class AISpawnAssistEditor : Editor
{
    ReorderableList ScriptList;

    void OnEnable()
    {
        ScriptList = new ReorderableList(serializedObject,
            serializedObject.FindProperty("ScriptsToSpawnWith"),
            true, true, true, true);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DisplayScript();

        DisplayAIType();

        DisplayScriptList();

        DisplayDragNDrop();

        //DrawDefaultInspector ();

        serializedObject.ApplyModifiedProperties();
    }

    void DisplayScriptProperties()
    {
        for(int i = 0; i < ScriptList.serializedProperty.arraySize; ++i)
        {
            SerializedProperty scriptObj = ScriptList.serializedProperty.GetArrayElementAtIndex(i); 
            EditorGUILayout.PropertyField(scriptObj);
            /*SerializedProperty iterator = ScriptList.serializedProperty.GetArrayElementAtIndex(i).serializedObject.GetIterator();
            while (iterator.Next(true))
            {
                iterator.NextVisible(false);
                EditorGUILayout.PropertyField(iterator, true);
            }*/
        }
    }

    void DisplayAIType()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("TypeOfAI"));
    }

    void DisplayDragNDrop()
    {
        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag scripts here");

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                {
                    return;
                }

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (Object obj in DragAndDrop.objectReferences)
                    {
                        if (obj is MonoScript)
                        {
                            ((AISpawnAssist)target).ScriptsToSpawnWith.Add((MonoScript)obj);
                        }
                    }
                }
                break;
        }

    }

    void DisplayScriptList()
    {
        ScriptList.DoLayoutList();
        ScriptList.drawElementCallback = (Rect rect, int index, bool isActive, bool isfocused) =>
        {
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), ScriptList.serializedProperty.GetArrayElementAtIndex(index));
        };
        ScriptList.onRemoveCallback = (ReorderableList list) =>
        {
            int arraySize = list.serializedProperty.arraySize;
            list.serializedProperty.DeleteArrayElementAtIndex(list.index);
            if(list.serializedProperty.arraySize == arraySize)
            {
                list.serializedProperty.DeleteArrayElementAtIndex(list.index);
            }
        };
    }

    void DisplayScript()
    {
        SerializedProperty iterator = serializedObject.GetIterator();
        iterator.NextVisible(true);
        GUI.enabled = false;
        EditorGUILayout.PropertyField(iterator, true);
        GUI.enabled = true;
    }

}
#endif