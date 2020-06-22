using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class AssetHandler
{
    [OnOpenAsset()]
    public static bool OpenEditor(int instanceId, int line)
    {
        UtilityAIAgent obj = EditorUtility.InstanceIDToObject(instanceId) as UtilityAIAgent;
        if(obj != null)
        {
            UtilityAIAgentEditorWindow.Open(obj);
            return true;
        }
        return false;
    }
}

[CustomEditor(typeof(UtilityAIAgent))]
public class UtilityAIAgentCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Open Utility AI Editor"))
        {
            UtilityAIAgentEditorWindow.Open((UtilityAIAgent)target);
        }
    }
}
