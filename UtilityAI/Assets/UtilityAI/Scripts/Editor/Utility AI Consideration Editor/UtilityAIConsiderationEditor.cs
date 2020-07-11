using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class UtilityAIConsiderationEditor : VisualElement
{
    UtilityAIConsideration consideration;
    public UtilityAIActionEditor utilityAIActionEditor;

    Foldout considerationContainerFoldout;
    VisualElement responseCurveContainer;

    public UtilityAIConsiderationEditor(UtilityAIActionEditor utilityAIActionEditor, UtilityAIConsideration consideration)
    {
        this.consideration = consideration;
        this.utilityAIActionEditor = utilityAIActionEditor;

        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UtilityAI/Scripts/Editor/Utility AI Consideration Editor/UtilityAIConsiderationEditor.uxml");
        visualTree.CloneTree(this);

        StyleSheet stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UtilityAI/Scripts/Editor/Utility AI Consideration Editor/UtilityAIConsiderationEditor.uss");
        this.styleSheets.Add(stylesheet);

        this.AddToClassList("utilityAIActionEditor");

        Button moveUpButton = this.Query<Button>("moveUpButton").First();
        moveUpButton.BringToFront();
        moveUpButton.clickable.clicked += MoveConsiderationUp;

        Button moveDownButton = this.Query<Button>("moveDownButton").First();
        moveDownButton.BringToFront();
        moveDownButton.clickable.clicked += MoveConsiderationDown;

        Button deleteButton = this.Query<Button>("deleteButton").First();
        deleteButton.BringToFront();
        deleteButton.clickable.clicked += DeleteConsideration;

        considerationContainerFoldout = this.Query<Foldout>("consideration").First();
        considerationContainerFoldout.text = consideration.name + ": ";

        considerationContainerFoldout.Query<Toggle>().First().AddToClassList("considerationFoldout");

        responseCurveContainer = this.Query<VisualElement>("responseCurve").First();

        TextField nameField = this.Query<TextField>("considerationName").First();
        nameField.value = consideration.name;
        nameField.RegisterCallback<ChangeEvent<string>>(
            e =>
            {
                if (e.newValue != "")
                {
                    consideration.name = (string)e.newValue;
                    considerationContainerFoldout.text = (string)e.newValue + ":";
                }
            }
        );

        Toggle enabledField = this.Query<Toggle>("considerationEnabled").First();
        enabledField.value = consideration.enabled;
        enabledField.RegisterCallback<ChangeEvent<bool>>(
            e =>
            {
                consideration.enabled = (bool)e.newValue;
            }
        );

        VisualElement inputFieldContainer = this.Query<VisualElement>("considerationInput").First();
        List<GameObject> considerationInputsPrefabs = FindPrefabsWithComponent<UtilityAIConsiderationInput>();
        List<UtilityAIConsiderationInput> considerationInputs = new List<UtilityAIConsiderationInput>();
        foreach(GameObject prefab in considerationInputsPrefabs)
        {
            considerationInputs.Add(prefab.GetComponent<UtilityAIConsiderationInput>());
        }
        if(consideration.considerationInput == null)
        {
            consideration.considerationInput = considerationInputs[0];
        }
        Debug.Log(consideration.considerationInput);
        int currentIndex = considerationInputs.IndexOf(consideration.considerationInput);
        PopupField<UtilityAIConsiderationInput> inputFieldPopup = new PopupField<UtilityAIConsiderationInput>("Consideration Inputs: ", considerationInputs, currentIndex);
        inputFieldPopup.RegisterCallback<ChangeEvent<UtilityAIConsiderationInput>>(
            e =>
            {
                consideration.considerationInput = (UtilityAIConsiderationInput)e.newValue;
            }
        );
        inputFieldContainer.Add(inputFieldPopup);

        UpdateResponseCurve();
    }

    public void UpdateResponseCurve()
    {
        responseCurveContainer.Clear();
        ResponseCurveEditor responseCurveEditor = new ResponseCurveEditor(this, consideration.responseCurve);
        responseCurveContainer.Add(responseCurveEditor);
    }

    private void MoveConsiderationUp()
    {
        if (utilityAIActionEditor.utilityAIAgentEditor.GetType() == typeof(UtilityAIAgentEditor))
        {
            UtilityAIAgentEditor editorWindow = (UtilityAIAgentEditor)utilityAIActionEditor.utilityAIAgentEditor;
            int index = utilityAIActionEditor.action.considerations.IndexOf(consideration);
            editorWindow.SwapItemsInCollection(utilityAIActionEditor.action.considerations, index, index - 1);
        }
    }

    private void MoveConsiderationDown()
    {
        if (utilityAIActionEditor.utilityAIAgentEditor.GetType() == typeof(UtilityAIAgentEditor))
        {
            UtilityAIAgentEditor editorWindow = (UtilityAIAgentEditor)utilityAIActionEditor.utilityAIAgentEditor;
            int index = utilityAIActionEditor.action.considerations.IndexOf(consideration);
            editorWindow.SwapItemsInCollection(utilityAIActionEditor.action.considerations, index, index + 1);
        }
    }

    private void DeleteConsideration()
    {
        if (EditorUtility.DisplayDialog("Delete Consideration", "Are you sure you want to remove this consideration from the action?", "Delete", "Cancel"))
        {
            if (utilityAIActionEditor.utilityAIAgentEditor.GetType() == typeof(UtilityAIAgentEditor))
            {
                UtilityAIAgentEditor editorWindow = (UtilityAIAgentEditor)utilityAIActionEditor.utilityAIAgentEditor;
                editorWindow.RemoveItemFromCollection(utilityAIActionEditor.action.considerations, consideration);
            }
        }
    }

    public List<GameObject> FindPrefabsWithComponent<T>() where T : UnityEngine.Object
    {
        List<GameObject> assets = new List<GameObject>();
        string[] guids = AssetDatabase.FindAssets("t:prefab");
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (asset != null)
            {
                assets.Add(asset);
            }
        }
        return assets;
    }

    /*private void GetConsiderationInputScripts()
    {
        List<GameObject> considerationInputs = FindPrefabWithComponent<UtilityAIConsiderationInput>();
        List<string> considerationInputNames = new List<string>();

        for (int i = 0; i < considerationInputs.Count; i++)
        {
            considerationInputNames.Add(considerationInputs[i].name);
        }
        if (considerationInputs.Count <= 0)
        {
            considerationInputNames = new List<string> { "No consideration inputs found" };
        }

        for (int i = 0; i < considerationInputs.Count; i++)
        {
            if (currentProperty.FindPropertyRelative("considerationInput").objectReferenceValue == considerationInputs[i].GetComponent<UtilityAIConsiderationInput>())
            {
                selectedConsiderationInput = i;
            }
        }
        int newSelectedConsiderationInput = EditorGUILayout.Popup(selectedConsiderationInput, considerationInputNames.ToArray());
        if (newSelectedConsiderationInput != selectedConsiderationInput)
        {
            int selection = newSelectedConsiderationInput;
            if (considerationInputNames[0] == "None" || considerationInputNames[0] == "Missing")
            {
                selection = selection - 1;
            }
            if (selection >= 0)
            {
                ChangeConsiderationInput(considerationInputs, selection);
            }
        }
        selectedConsiderationInput = newSelectedConsiderationInput;
    }*/
}