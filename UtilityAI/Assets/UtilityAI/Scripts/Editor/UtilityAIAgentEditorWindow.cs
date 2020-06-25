﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public class UtilityAIAgentEditorWindow : ExtendedEditorWindow
{
    public int selectedConsiderationInput = 0;
    private UtilityAIAgent serializedObjectTargetBackup;
    
    public static void Open(UtilityAIAgent agent)
    {
        UtilityAIAgentEditorWindow window = GetWindow<UtilityAIAgentEditorWindow>("AI Editor");
        window.serializedObject = new SerializedObject(agent);
    }

    private void OnGUI()
    {
        //If the reference to the serialised object has been lost, recreate it from the backup:
        if(serializedObject == null && serializedObjectTargetBackup != null)
        {
            serializedObject = new SerializedObject(serializedObjectTargetBackup);
        }
        //If necessary create a backup of the target object:
        if(serializedObjectTargetBackup == null )
        {
            serializedObjectTargetBackup = (UtilityAIAgent) serializedObject.targetObject;
        }

        currentProperty = serializedObject.FindProperty("actions");
        //Display the Actions available to the agent:
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));
        EditorGUILayout.LabelField("Actions:", EditorStyles.boldLabel);
        DrawSidebar(currentProperty, 1);
        EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(150));
        AddListItem(currentProperty, "Add Action");
        RemoveListItem(currentProperty, "Remove Action");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        //If an action has been selected, display its considerations:
        EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
        if(selectedProperty[0] != null)
        {
            EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));
            DrawSelectedActionProperties(selectedProperty[0]);
            EditorGUILayout.LabelField("Considerations:", EditorStyles.boldLabel);
            DrawSidebar(selectedProperty[0].FindPropertyRelative("considerations"), 2);
            EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(150));
            AddListItem(selectedProperty[0].FindPropertyRelative("considerations"), "Add Consideration");
            RemoveListItem(selectedProperty[0].FindPropertyRelative("considerations"), "Remove Consideration");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            //If a consideration has been selected, and it is a consideration for the currently selected action, display its parameters:
            EditorGUILayout.BeginVertical("box");
            if(selectedProperty[1] != null && selectedProperty[1].propertyPath.Contains(selectedProperty[0].propertyPath))
            {
                DrawSelectedConsiderationProperties(selectedProperty[1]);
            }
            else
            {
                EditorGUILayout.LabelField("Select a consideration from the list");
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.LabelField("Select an action from the list");
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        //Apply any changes made:
        Apply();
    }
    void DrawSelectedActionProperties(SerializedProperty prop)
    {
        currentProperty = prop;
        EditorGUILayout.BeginHorizontal("box");
        DrawField("name", true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal("box");
        DrawField("enabled", true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal("box");
        DrawField("action", true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal("box");
        DrawField("interruptable", true);
        EditorGUILayout.EndHorizontal();
    }
    void DrawSelectedConsiderationProperties(SerializedProperty prop)
    {
        currentProperty = prop;
        EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
        EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
        DrawField("name", true);
        DrawField("enabled", true);
        EditorGUILayout.EndHorizontal();
        DisplayConsiderationInputSelection();
        EditorGUILayout.EndVertical();

        currentProperty = currentProperty.FindPropertyRelative("responseCurve");
        EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandWidth(true));
        EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandWidth(true));
        EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
        DrawField("curveType", true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
        EditorGUILayout.BeginHorizontal("box", GUILayout.MaxWidth(300));
        DrawField("xShift", true);
        DrawField("yShift", true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal("box", GUILayout.MaxWidth(300));
        DrawField("slope", true);
        DrawField("exponential", true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
        currentProperty = prop;


        EditorGUILayout.BeginVertical("box");
        EditorGUIUtility.labelWidth = 0.1f;
        DrawField("responseCurve", true);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
        if(GUILayout.Button("Remove Consideration"))
        {   
            SerializedProperty considerations = selectedProperty[0].FindPropertyRelative("considerations");
            if(considerations != null)
            {
                for(int p = 0; p < considerations.arraySize; p++)
                {
                    if(currentProperty.propertyPath == considerations.GetArrayElementAtIndex(p).propertyPath)
                    {
                        considerations.DeleteArrayElementAtIndex(p);
                        break;
                    }
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    void DisplayConsiderationInputSelection()
    {
        //Get all the consideration inputs and make a list of their names:
        List<GameObject> considerationInputs = FindPrefabWithComponent<UtilityAIConsiderationInput>();
        List<string> considerationInputNames = new List<string>();
        if(currentProperty.FindPropertyRelative("considerationInput").objectReferenceValue == null)
        {
            if(currentProperty.FindPropertyRelative("considerationInput").objectReferenceInstanceIDValue != 0)
            {
                considerationInputNames.Add("Missing");                
            }
            else
            {
                considerationInputNames.Add("None");
            }
        }
        for(int i = 0; i < considerationInputs.Count; i++)
        {
            considerationInputNames.Add(considerationInputs[i].name);
        }
        if(considerationInputs.Count <= 0)
        {
            considerationInputNames = new List<string>{"No consideration inputs found"};
        }

        for(int i = 0; i < considerationInputs.Count; i++)
        {
            if(currentProperty.FindPropertyRelative("considerationInput").objectReferenceValue == considerationInputs[i].GetComponent<UtilityAIConsiderationInput>())
            {
                selectedConsiderationInput = i;
            }
        }
        int newSelectedConsiderationInput = EditorGUILayout.Popup(selectedConsiderationInput, considerationInputNames.ToArray()); 
        if(newSelectedConsiderationInput != selectedConsiderationInput)
        {
            int selection = newSelectedConsiderationInput;
            if(considerationInputNames[0] == "None" || considerationInputNames[0] == "Missing")
            {
                selection = selection - 1;
            }
            if(selection >= 0)
            {
                ChangeConsiderationInput(considerationInputs, selection);
            }
        }
        selectedConsiderationInput = newSelectedConsiderationInput;
    }

    void ChangeConsiderationInput(List<GameObject> considerationInputs, int selection)
    {
        currentProperty.FindPropertyRelative("considerationInput").objectReferenceValue = considerationInputs[selection];
    }
}
