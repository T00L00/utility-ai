using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class ExposedDelegateEditor : VisualElement
{
    UtilityAIActionEditor utilityAIActionEditor;
    ExposedDelegate exposedDelegate;

    Foldout delegateEntriesList;

    public ExposedDelegateEditor(UtilityAIActionEditor utilityAIActionEditor, ExposedDelegate exposedDelegate)
    {
        this.utilityAIActionEditor = utilityAIActionEditor;
        this.exposedDelegate = exposedDelegate;

        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UtilityAI/Scripts/Editor/Exposed Delegate Editor/ExposedDelegateEditor.uxml");
        visualTree.CloneTree(this);

        StyleSheet stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UtilityAI/Scripts/Editor/Exposed Delegate Editor/ExposedDelegateEditor.uss");
        this.styleSheets.Add(stylesheet);

        this.AddToClassList("exposedDelegateEditor");

        delegateEntriesList = this.Query<Foldout>("delegateEntries");

        UpdateDelegateEntries();

        Button btnAddDelegateEntry = this.Query<Button>("btnAddNew").First();
        btnAddDelegateEntry.clickable.clicked += AddDelegateEntry;
    }

    public void UpdateDelegateEntries()
    {
        delegateEntriesList.Clear();
        foreach (DelegateEntry delegateEntry in exposedDelegate.delegateEntries)
        {
            DelegateEntryEditor delegateEntryEditor = new DelegateEntryEditor(this, delegateEntry);
            delegateEntriesList.Add(delegateEntryEditor);
        }
    }

    private void AddDelegateEntry()
    {
        DelegateEntry delegateEntry = ScriptableObject.CreateInstance<DelegateEntry>();
        exposedDelegate.delegateEntries.Add(delegateEntry);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        UpdateDelegateEntries();
    }
}