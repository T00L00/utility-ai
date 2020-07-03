using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class ExposedDelegateEditor : VisualElement
{
    public UtilityAIActionEditor utilityAIActionEditor;
    public ExposedDelegate exposedDelegate;

    Foldout delegateEntriesList;
    VisualElement delegateEntriesContainer;

    public ExposedDelegateEditor(UtilityAIActionEditor utilityAIActionEditor, ExposedDelegate exposedDelegate)
    {
        this.utilityAIActionEditor = utilityAIActionEditor;
        this.exposedDelegate = exposedDelegate;

        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UtilityAI/Scripts/Editor/Exposed Delegate Editor/ExposedDelegateEditor.uxml");
        visualTree.CloneTree(this);

        StyleSheet stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UtilityAI/Scripts/Editor/Exposed Delegate Editor/ExposedDelegateEditor.uss");
        this.styleSheets.Add(stylesheet);

        this.AddToClassList("exposedDelegateEditor");

        Button btnAddDelegateEntry = this.Query<Button>("btnAddNewMethod").First();
        btnAddDelegateEntry.BringToFront();
        btnAddDelegateEntry.clickable.clicked += AddDelegateEntry;

        Button btnRemoveAllDelegateEntries = this.Query<Button>("btnRemoveAllMethods").First();
        btnRemoveAllDelegateEntries.BringToFront();
        btnRemoveAllDelegateEntries.clickable.clicked += RemoveAllDelegateEntries;

        delegateEntriesList = this.Query<Foldout>("delegateEntries");
        delegateEntriesList.Query<Toggle>().First().AddToClassList("delegateEntryFoldout");

        delegateEntriesContainer = delegateEntriesList.Query<VisualElement>("delegateEntriesContainer").First();

        UpdateDelegateEntries();
    }

    public void UpdateDelegateEntries()
    {
        delegateEntriesContainer.Clear();
        if (exposedDelegate != null)
        {
            foreach (DelegateEntry delegateEntry in exposedDelegate.delegateEntries)
            {
                DelegateEntryEditor delegateEntryEditor = new DelegateEntryEditor(this, delegateEntry);
                delegateEntriesContainer.Add(delegateEntryEditor);
            }
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

    private void RemoveAllDelegateEntries()
    {
        if (EditorUtility.DisplayDialog("Delete All Delegate Entries", "Are you sure you want to delete all of the entries for this delegate?", "Delete All", "Cancel"))
        {
            for (int i = exposedDelegate.delegateEntries.Count - 1; i >= 0; i--)
            {
                AssetDatabase.RemoveObjectFromAsset(exposedDelegate.delegateEntries[i]);
                exposedDelegate.delegateEntries.RemoveAt(i);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            UpdateDelegateEntries();
        }
    }
}