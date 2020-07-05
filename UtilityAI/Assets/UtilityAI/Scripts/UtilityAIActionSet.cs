using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UtilityAIActionSet : ScriptableObject
{
    [System.Serializable]
    public class ActionSetAction
    {
        public string name;
        public bool enabled;
        public List<ActionSetMethod> methods;
        public List<ActionSetConsideration> considerations;

        public ActionSetAction(string name, bool enabled)
        {
            this.name = name;
            this.enabled = enabled;
            methods = new List<ActionSetMethod>();
            considerations = new List<ActionSetConsideration>();
        }
    }

    [System.Serializable]
    public class ActionSetMethod
    {
        public string componentName;
        public string methodName;
        public SerializableObject[] parameters;

        public ActionSetMethod(string componentName, string methodName, SerializableObject[] parameters)
        {
            this.componentName = componentName;
            this.methodName = methodName;
            this.parameters = parameters;
        }
    }

    [System.Serializable]
    public class ActionSetConsideration
    {
        public string name;
        public bool enabled;
        public UtilityAIConsiderationInput considerationInput;
        public ActionSetResponseCurve responseCurve;

        public ActionSetConsideration(string name, bool enabled, UtilityAIConsiderationInput considerationInput, ResponseCurve responseCurve)
        {
            this.name = name;
            this.enabled = enabled;
            this.considerationInput = considerationInput;
            this.responseCurve = new ActionSetResponseCurve(responseCurve.curveType, responseCurve.slope, responseCurve.exponential, responseCurve.xShift, responseCurve.yShift);
        }
    }

    [System.Serializable]
    public class ActionSetResponseCurve
    {
        public CurveType curveType;
        public float slope;
        public float exponential;
        public float xShift;
        public float yShift;

        public ActionSetResponseCurve(CurveType curveType, float slope, float exponential, float xShift, float yShift)
        {
            this.curveType = curveType;
            this.slope = slope;
            this.exponential = exponential;
            this.xShift = xShift;
            this.yShift = yShift;
        } 
    }

    public List<ActionSetAction> actions = new List<ActionSetAction>();

    public static void SaveActionsAsSet(List<UtilityAIAction> actionsToSave, string name)
    {
        UtilityAIActionSet actionSet = ScriptableObject.CreateInstance<UtilityAIActionSet>();
        foreach(UtilityAIAction unsavedAction in actionsToSave)
        {
            ActionSetAction savedAction = new ActionSetAction(unsavedAction.name, unsavedAction.enabled);
            foreach(DelegateEntry unsavedMethod in unsavedAction.action.delegateEntries)
            { 
                ActionSetMethod savedMethod = new ActionSetMethod(unsavedMethod.Target.GetType().ToString().Replace("UnityEngine.", ""), unsavedMethod.Method.Name, unsavedMethod.Parameters);
                savedAction.methods.Add(savedMethod);
            }
            foreach(UtilityAIConsideration unsavedConsideration in unsavedAction.considerations)
            {
                ActionSetConsideration savedConsideration = new ActionSetConsideration(unsavedConsideration.name, unsavedConsideration.enabled, unsavedConsideration.considerationInput, unsavedConsideration.responseCurve);
                savedAction.considerations.Add(savedConsideration);
            }
            actionSet.actions.Add(savedAction);
        }
        AssetDatabase.CreateAsset(actionSet, AssetDatabase.GenerateUniqueAssetPath("Assets/" + name + ".asset"));
    }

    public static List<UtilityAIAction> LoadActionsFromSet(UtilityAIActionSet actionSet, GameObject TargetGO)
    {
        List<UtilityAIAction> loadedActions = new List<UtilityAIAction>();
        foreach(ActionSetAction savedAction in actionSet.actions)
        {
            UtilityAIAction loadedAction = ScriptableObject.CreateInstance<UtilityAIAction>();
            loadedAction.name = savedAction.name;
            loadedAction.enabled = savedAction.enabled;
            foreach(ActionSetMethod savedMethod in savedAction.methods)
            {
                DelegateEntry loadedDelegateEntry = ScriptableObject.CreateInstance<DelegateEntry>();
                loadedDelegateEntry.TargetGO = TargetGO;
                loadedDelegateEntry.Target = loadedDelegateEntry.TargetGO.GetComponent(savedMethod.componentName);
                Debug.Log(loadedDelegateEntry.Target);
                loadedDelegateEntry.Method = loadedDelegateEntry.Target.GetType().GetMethod(savedMethod.methodName);
                loadedDelegateEntry.Parameters = savedMethod.parameters;
                loadedAction.action.delegateEntries.Add(loadedDelegateEntry);
            }
            foreach(ActionSetConsideration savedConsideration in savedAction.considerations)
            {
                UtilityAIConsideration loadedConsideration = ScriptableObject.CreateInstance<UtilityAIConsideration>();
                loadedConsideration.name = savedConsideration.name;
                loadedConsideration.enabled = savedConsideration.enabled;
                loadedConsideration.considerationInput = savedConsideration.considerationInput;
                ActionSetResponseCurve savedResponseCurve = savedConsideration.responseCurve;
                loadedConsideration.responseCurve = new ResponseCurve(savedResponseCurve.curveType, savedResponseCurve.slope, savedResponseCurve.exponential, savedResponseCurve.xShift, savedResponseCurve.yShift);
                loadedAction.considerations.Add(loadedConsideration);
            }
            loadedActions.Add(loadedAction);
        }
        return loadedActions;
    }
}
