using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(UtilityAIActionSet))]
public class UtilityAIActionSetEditor : Editor
{
    public UtilityAIActionSet utilityAIAgent;
    VisualElement rootElement;

    Foldout actionList;
    VisualElement actionContainer;

    public void OnEnable()
    {
        utilityAIAgent = (UtilityAIActionSet)target;
        rootElement = new VisualElement();

        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UtilityAI/Scripts/Editor/Utility AI Action Set Editor/UtilityAIActionSetEditor.uxml");
        visualTree.CloneTree(rootElement);

        StyleSheet stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UtilityAI/Scripts/Editor/Utility AI Action Set Editor/UtilityAIActionSetEditor.uss");
        rootElement.styleSheets.Add(stylesheet);
    }

    public override VisualElement CreateInspectorGUI()
    {
        rootElement.AddToClassList("actionSetContainer");

        actionList = rootElement.Query<Foldout>("actionList").First();
        actionContainer = actionList.Query<VisualElement>("actionsContainer").First();

        //Set up the Inheriting Object Field Logic:
        ObjectField inheritingObjectField = rootElement.Query<ObjectField>("inheritingObjectField");
        inheritingObjectField.objectType = typeof(GameObject);
        if (utilityAIAgent.inheritingGameObject != null && utilityAIAgent.inheritingGameObject.GetComponent<UtilityAIAgent>() != null && utilityAIAgent.inheritingGameObject.GetComponent<UtilityAIAgent>().actionSet == utilityAIAgent)
        {
            inheritingObjectField.value = utilityAIAgent.inheritingGameObject;
        }
        else
        {
            inheritingObjectField.value = utilityAIAgent.inheritingGameObject = SearchForObjectUsingActionSet();
            if(utilityAIAgent.inheritingGameObject == null)
            {
                if(EditorUtility.DisplayDialog("No agents are using this action set!", "No agents are currently using this action set, would you like to delete it?", "Yes", "No"))
                {
                    utilityAIAgent.DeleteFromFiles();
                }
            }
        }
        inheritingObjectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(
            e =>
            {
                HandleInheritingObjectChange(inheritingObjectField, e);
            }
        );

        actionList.Query<Toggle>().First().AddToClassList("actionListFoldout");

        Button btnAddAction = rootElement.Query<Button>("btnAddNewAction").First();
        btnAddAction.BringToFront();
        btnAddAction.clickable.clicked += AddAction;

        Button btnDeleteAllActions = rootElement.Query<Button>("btnRemoveAll").First();
        btnDeleteAllActions.BringToFront();
        btnDeleteAllActions.clickable.clicked += RemoveAllActions;

        UpdateActions();
        return rootElement;
    }

    public void UpdateActions()
    {
        actionContainer.Clear();
        foreach (UtilityAIAction action in utilityAIAgent.actions)
        {
            UtilityAIActionEditor utilityAIActionEditor = new UtilityAIActionEditor(this, action);
            actionContainer.Add(utilityAIActionEditor);
        }
    }

    private void AddAction()
    {
        UtilityAIAction action = new UtilityAIAction();
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
                utilityAIAgent.actions.RemoveAt(i);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            UpdateActions();
        }
    }

    public void HandleInheritingObjectChange(ObjectField inheritingObjectField, ChangeEvent<UnityEngine.Object> e)
    {
        GameObject newValue = (GameObject)e.newValue;
        if (newValue != null)
        {
            UtilityAIAgent newValueAgent = newValue.GetComponent<UtilityAIAgent>();
            //If the new object has a Utility AI Agent script attached, and that agent is using this action set, update the value:
            if (newValueAgent != null && newValueAgent.actionSet == utilityAIAgent)
            {
                utilityAIAgent.inheritingGameObject = (GameObject)e.newValue;
            }
            //Else reset the object field to the previous value:
            else
            {
                inheritingObjectField.value = (GameObject)e.previousValue;
            }
        }
        //If the new value is null, and the last value was not null, reset the field to the previous value:
        else if (e.newValue == null && e.previousValue != null)
        {
            inheritingObjectField.value = (GameObject)e.previousValue;
        }
        //Update the UI:
        UpdateActions();
    }

    public GameObject SearchForObjectUsingActionSet()
    {
        List<GameObject> allGOs = Resources.FindObjectsOfTypeAll<GameObject>().ToList();
        foreach(GameObject GO in allGOs)
        {
            UtilityAIAgent agent = GO.GetComponent<UtilityAIAgent>();
            if(agent != null && agent.actionSet == utilityAIAgent)
            {
                return agent.gameObject;
            }
        }
        return null;
    }
}