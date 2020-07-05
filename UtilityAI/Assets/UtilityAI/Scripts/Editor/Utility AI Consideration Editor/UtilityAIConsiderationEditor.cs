using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class UtilityAIConsiderationEditor : VisualElement
{
    UtilityAIConsideration consideration;
    UtilityAIActionEditor utilityAIActionEditor;

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
                    considerationContainerFoldout.text = (string)e.newValue;
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

        ObjectField inputField = this.Query<ObjectField>("considerationInput").First();
        inputField.objectType = typeof(UtilityAIConsiderationInput);
        inputField.value = consideration.considerationInput;
        inputField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(
            e =>
            {
                consideration.considerationInput = (UtilityAIConsiderationInput)e.newValue;
            }
        );

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
        int index = utilityAIActionEditor.action.considerations.IndexOf(consideration);
        utilityAIActionEditor.utilityAIAgentEditor.SwapItemsInCollection(utilityAIActionEditor.action.considerations, index, index - 1);
    }

    private void MoveConsiderationDown()
    {
        int index = utilityAIActionEditor.action.considerations.IndexOf(consideration);
        utilityAIActionEditor.utilityAIAgentEditor.SwapItemsInCollection(utilityAIActionEditor.action.considerations, index, index + 1);
    }

    private void DeleteConsideration()
    {
        if (EditorUtility.DisplayDialog("Delete Consideration", "Are you sure you want to remove this consideration from the action?", "Delete", "Cancel"))
        {
            utilityAIActionEditor.utilityAIAgentEditor.RemoveItemFromCollection(utilityAIActionEditor.action.considerations, consideration);
        }
    }
}