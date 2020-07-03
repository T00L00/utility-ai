using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class UtilityAIActionEditor : VisualElement
{
    public UtilityAIAction action;
    public UtilityAIAgentEditor utilityAIAgentEditor;

    Foldout actionContainerFoldout;
    VisualElement actionDelegateContainer;
    Foldout considerationsFoldout;
    VisualElement considerationsContainer;

    public UtilityAIActionEditor(UtilityAIAgentEditor utilityAIAgentEditor, UtilityAIAction action)
    {
        this.utilityAIAgentEditor = utilityAIAgentEditor;
        this.action = action;

        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UtilityAI/Scripts/Editor/Utility AI Action Editor/UtilityAIActionEditor.uxml");
        visualTree.CloneTree(this);

        StyleSheet stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UtilityAI/Scripts/Editor/Utility AI Action Editor/UtilityAIActionEditor.uss");
        this.styleSheets.Add(stylesheet);

        this.AddToClassList("utilityAIActionEditor");

        Button moveUpButton = this.Query<Button>("moveUpButton").First();
        moveUpButton.BringToFront();
        moveUpButton.clickable.clicked += MoveActionUp;

        Button moveDownButton = this.Query<Button>("moveDownButton").First();
        moveDownButton.BringToFront();
        moveDownButton.clickable.clicked += MoveActionDown;

        Button deleteButton = this.Query<Button>("deleteButton").First();
        deleteButton.BringToFront();
        deleteButton.clickable.clicked += DeleteAction;


        actionContainerFoldout = this.Query<Foldout>("action").First();
        actionContainerFoldout.text = action.name;

        actionContainerFoldout.Query<Toggle>().First().AddToClassList("actionFoldout");

        actionDelegateContainer = actionContainerFoldout.Query<VisualElement>("actionContainer").First();

        considerationsFoldout = this.Query<Foldout>("considerationsList").First();
        considerationsFoldout.Query<Toggle>().First().AddToClassList("considerationsFoldout");
        considerationsContainer = considerationsFoldout.Query<VisualElement>("considerationsContainer").First();

        TextField nameField = this.Query<TextField>("actionName").First();
        nameField.value = action.name;
        nameField.RegisterCallback<ChangeEvent<string>>(
            e =>
            {
                action.name = (string)e.newValue;
                actionContainerFoldout.text = (string)e.newValue;
                EditorUtility.SetDirty(action);
            }
        );

        Toggle enabledField = this.Query<Toggle>("actionEnabled").First();
        enabledField.value = action.enabled;
        enabledField.RegisterCallback<ChangeEvent<bool>>(
            e =>
            {
                action.enabled = (bool)e.newValue;
                EditorUtility.SetDirty(action);
            }
        );

        UpdateAction();

        UpdateConsiderations();

        Button btnAddConsideration = this.Query<Button>("btnAddNewConsideration").First();
        btnAddConsideration.BringToFront();
        btnAddConsideration.clickable.clicked += AddConsideration;

        Button btnRemoveAllConsiderations = this.Query<Button>("btnRemoveAllConsiderations").First();
        btnRemoveAllConsiderations.BringToFront();
        btnRemoveAllConsiderations.clickable.clicked += RemoveAllConsiderations;

    }

    public void UpdateAction()
    {
        ExposedDelegateEditor exposedDelegateEditor = new ExposedDelegateEditor(this, action.action);
        actionDelegateContainer.Add(exposedDelegateEditor);
    }

    public void UpdateConsiderations()
    {
        considerationsContainer.Clear();
        if (action.considerations != null)
        {
            foreach (UtilityAIConsideration consideration in action.considerations)
            {
                UtilityAIConsiderationEditor utilityAIConsiderationEditor = new UtilityAIConsiderationEditor(this, consideration);
                considerationsContainer.Add(utilityAIConsiderationEditor);
            }
        }
    }

    private void MoveActionUp()
    {
        int index = utilityAIAgentEditor.utilityAIAgent.actions.IndexOf(action);
        utilityAIAgentEditor.SwapItemsInCollection(utilityAIAgentEditor.utilityAIAgent.actions, index, index - 1);
    }

    private void MoveActionDown()
    {
        int index = utilityAIAgentEditor.utilityAIAgent.actions.IndexOf(action);
        utilityAIAgentEditor.SwapItemsInCollection(utilityAIAgentEditor.utilityAIAgent.actions, index, index + 1);
    }

    private void DeleteAction()
    {
        if (EditorUtility.DisplayDialog("Delete Action", "Are you sure you want to remove this action from the agent?", "Delete", "Cancel"))
        {
            utilityAIAgentEditor.RemoveItemFromCollection(utilityAIAgentEditor.utilityAIAgent.actions, action);
        }
    }

    private void AddConsideration()
    {
        UtilityAIConsideration consideration = ScriptableObject.CreateInstance<UtilityAIConsideration>();
        action.considerations.Add(consideration);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        UpdateConsiderations();
    }

    private void RemoveAllConsiderations()
    {
        if (EditorUtility.DisplayDialog("Delete All Considerations", "Are you sure you want to delete all of the considerations for this action?", "Delete All", "Cancel"))
        {
            for (int i = action.considerations.Count - 1; i >= 0; i--)
            {
                AssetDatabase.RemoveObjectFromAsset(action.considerations[i]);
                action.considerations.RemoveAt(i);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            UpdateConsiderations();
        }
    }
}