using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExtendedEditorWindow : EditorWindow
{
    protected SerializedObject serializedObject;
    protected SerializedProperty currentProperty;

    private List<string> selectedPropertyPath = new List<string>();
    protected List<SerializedProperty> selectedProperty = new List<SerializedProperty>();
    
    protected void DrawProperties(SerializedProperty prop, bool drawChildren)
    {
        string lastPropPath = string.Empty;
        foreach(SerializedProperty p in prop)
        {
            if(p.isArray && p.propertyType == SerializedPropertyType.Generic)
            {
                EditorGUILayout.BeginHorizontal();
                p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                EditorGUILayout.EndHorizontal();
                if(p.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    DrawProperties(p, drawChildren);
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                if(!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath))
                {
                    continue;
                }
                lastPropPath = p.propertyPath;
                EditorGUILayout.PropertyField(p, drawChildren);
            }
        }
    }

    protected void DrawSidebar(SerializedProperty prop, int depth)
    {   
        if(selectedPropertyPath.Count < depth)
        {
            selectedPropertyPath.Add(null);
        }
        if(selectedProperty.Count < depth)
        {
            selectedProperty.Add(null);
        }
        foreach(SerializedProperty p in prop)
        {
            if(GUILayout.Button(p.displayName))
            {
                selectedPropertyPath[depth - 1] = p.propertyPath;
            }
        }
        if(!string.IsNullOrEmpty(selectedPropertyPath[depth - 1]))
        {
            selectedProperty[depth - 1] = serializedObject.FindProperty(selectedPropertyPath[depth - 1]);
        }
    }

    protected void DrawField(string propName, bool relative)
    {
        if(relative && currentProperty != null)
        {
            EditorGUILayout.PropertyField(currentProperty.FindPropertyRelative(propName), true);
        }
        else if(serializedObject != null)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(propName), true);
        }
    } 

    protected void AddListItem(SerializedProperty prop, string buttonText)
    {
        if(GUILayout.Button(buttonText))
        {
            if(prop.isArray)
            {
                prop.arraySize++;
            }
        }
    }

    protected void RemoveListItem(SerializedProperty prop, string buttonText)
    {
        if(GUILayout.Button(buttonText))
        {
            if(prop.isArray)
            {
                if(selectedPropertyPath.Contains(prop.GetArrayElementAtIndex(prop.arraySize - 1).propertyPath))
                {
                    selectedProperty[selectedPropertyPath.IndexOf(prop.GetArrayElementAtIndex(prop.arraySize - 1).propertyPath)] = null;
                    selectedPropertyPath[selectedPropertyPath.IndexOf(prop.GetArrayElementAtIndex(prop.arraySize - 1).propertyPath)] = null;
                }
                prop.arraySize--;
            }
        }
    }

    protected void Apply()
    {
        serializedObject.ApplyModifiedProperties();
    }

    public List<GameObject> FindPrefabWithComponent<T>() where T : UnityEngine.Object
    {
        List<GameObject> assets = new List<GameObject>();
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        for( int i = 0; i < guids.Length; i++ )
        {
            string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if(asset != null && (asset.GetComponent<T>() != null))
            {
                assets.Add(asset);
            }
        }
        return assets;
    }
}
