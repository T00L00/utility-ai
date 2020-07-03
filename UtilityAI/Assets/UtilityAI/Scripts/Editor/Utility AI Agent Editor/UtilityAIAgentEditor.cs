using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


[CustomEditor(typeof(UtilityAIAgent))]
public class UtilityAIAgentEditor : Editor
{
    public UtilityAIAgent utilityAIAgent;
    VisualElement rootElement;

    Foldout actionList;
    VisualElement actionContainer;

    public void OnEnable()
    {
        utilityAIAgent = (UtilityAIAgent)target;
        rootElement = new VisualElement();

        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UtilityAI/Scripts/Editor/Utility AI Agent Editor/UtilityAIAgentEditor.uxml");
        visualTree.CloneTree(rootElement);

        StyleSheet stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UtilityAI/Scripts/Editor/Utility AI Agent Editor/UtilityAIAgentEditor.uss");
        rootElement.styleSheets.Add(stylesheet);
    }

    public override VisualElement CreateInspectorGUI()
    {
        actionList = rootElement.Query<Foldout>("actionList").First();
        actionContainer = actionList.Query<VisualElement>("actionsContainer").First();

        actionList.Query<Toggle>().First().AddToClassList("actionListFoldout");

        UpdateActions();

        Button btnAddAction = rootElement.Query<Button>("btnAddNewAction").First();
        btnAddAction.BringToFront();
        btnAddAction.clickable.clicked += AddAction;

        Button btnDeleteAllActions = rootElement.Query<Button>("btnRemoveAll").First();
        btnDeleteAllActions.BringToFront();
        btnDeleteAllActions.clickable.clicked += RemoveAllActions;

        return rootElement;
    }

    public void UpdateActions()
    {
        actionContainer.Clear();
        foreach(UtilityAIAction action in utilityAIAgent.actions)
        {
            UtilityAIActionEditor utilityAIActionEditor = new UtilityAIActionEditor(this, action);
            actionContainer.Add(utilityAIActionEditor);
        }
    }

    private void AddAction()
    {
        UtilityAIAction action = ScriptableObject.CreateInstance<UtilityAIAction>();
        utilityAIAgent.actions.Add(action);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        UpdateActions();
    }

    private void RemoveAllActions()
    {
        if (EditorUtility.DisplayDialog("Delete All Actions", "Are you sure you want to delete all of the actions this agent has?", "Delete All", "Cancel"))
        {
            for (int i = utilityAIAgent.actions.Count - 1; i >= 0; i--)
            {
                AssetDatabase.RemoveObjectFromAsset(utilityAIAgent.actions[i]);
                utilityAIAgent.actions.RemoveAt(i);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            UpdateActions();
        }
    }

    public void SwapItemsInCollection<T>(List<T> collection, int index1, int index2)
    {
        if (index1 >= 0 && index1 < collection.Count && index2 >= 0 && index2 < collection.Count)
        {
            T temp = collection[index1];
            collection[index1] = collection[index2];
            collection[index2] = temp;
            UpdateActions();
        }
    }

    public void RemoveItemFromCollection<T>(List<T> collection, T itemToRemove)
    {
        collection.Remove(itemToRemove);
        AssetDatabase.RemoveObjectFromAsset(itemToRemove as UnityEngine.Object);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        UpdateActions();
    }
}