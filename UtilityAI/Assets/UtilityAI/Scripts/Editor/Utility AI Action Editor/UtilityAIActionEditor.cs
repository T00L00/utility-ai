using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class UtilityAIActionEditor : VisualElement
{
    UtilityAIAction action;
    UtilityAIAgentEditor utilityAIAgentEditor;

    Foldout actionContainerFoldout;

    public UtilityAIActionEditor(UtilityAIAgentEditor utilityAIAgentEditor, UtilityAIAction action)
    {
        this.utilityAIAgentEditor = utilityAIAgentEditor;
        this.action = action;

        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UtilityAI/Scripts/Editor/Utility AI Action Editor/UtilityAIActionEditor.uxml");
        visualTree.CloneTree(this);

        StyleSheet stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UtilityAI/Scripts/Editor/Utility AI Action Editor/UtilityAIActionEditor.uss");
        this.styleSheets.Add(stylesheet);

        this.AddToClassList("utilityAIActionEditor");

        actionContainerFoldout = this.Query<Foldout>("action").First();
        actionContainerFoldout.text = action.name;

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

        Button addConsiderationButton = new Button(AddConsideration) { text = "Add new Consideration" };
        actionContainerFoldout.Add(addConsiderationButton);
    }

    public void UpdateAction()
    {
        ExposedDelegateEditor exposedDelegateEditor = new ExposedDelegateEditor(this, action.action);
        actionContainerFoldout.Add(exposedDelegateEditor);
    }

    public void UpdateConsiderations()
    {
        Foldout considerationFoldout = new Foldout();
        considerationFoldout.text = "Considerations: ";
        foreach (UtilityAIConsideration consideration in action.considerations)
        {
            UtilityAIConsiderationEditor utilityAIConsiderationEditor = new UtilityAIConsiderationEditor(this, consideration);
            considerationFoldout.Add(utilityAIConsiderationEditor);
        }
        actionContainerFoldout.Add(considerationFoldout);
    }

    private void AddConsideration()
    {
        UtilityAIConsideration consideration = ScriptableObject.CreateInstance<UtilityAIConsideration>();
        action.considerations.Add(consideration);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        UpdateConsiderations();
    }
}