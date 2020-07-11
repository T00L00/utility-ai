using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UtilityAIActionSet : ScriptableObject
{
    public List<UtilityAIAction> actions = new List<UtilityAIAction>();
    public GameObject inheritingGameObject;

    public void DeleteFromFiles()
    {
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(this));
    }

    public static void SaveActionsAsSet(List<UtilityAIAction> actionsToSave, GameObject invoker)
    {
        UtilityAIActionSet actionSet = ScriptableObject.CreateInstance<UtilityAIActionSet>();
        actionSet.actions = actionsToSave;
        actionSet.inheritingGameObject = invoker;
        invoker.GetComponent<UtilityAIAgent>().actionSet = actionSet;
        AssetDatabase.CreateAsset(actionSet, AssetDatabase.GenerateUniqueAssetPath("Assets/" + invoker.name + ".asset"));
    }
}
