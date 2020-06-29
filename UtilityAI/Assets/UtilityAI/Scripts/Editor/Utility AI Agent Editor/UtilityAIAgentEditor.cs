using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


[CustomEditor(typeof(UtilityAIAgent))]
public class UtilityAIAgentEditor : Editor
{
    UtilityAIAgent utilityAIAgent;
    VisualElement rootElement;

    Foldout actionList;

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

        UpdateActions();

        Button btnAddAction = rootElement.Query<Button>("btnAddNew").First();
        btnAddAction.clickable.clicked += AddAction;

        return rootElement;
    }

    public void UpdateActions()
    {
        actionList.Clear();
        foreach(UtilityAIAction action in utilityAIAgent.actions)
        {
            UtilityAIActionEditor utilityAIActionEditor = new UtilityAIActionEditor(this, action);
            actionList.Add(utilityAIActionEditor);
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
}