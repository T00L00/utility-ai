using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UtilityAIAction
{
    public string name;
    public bool enabled = true;
    public ExposedDelegate action;
    public List<UtilityAIConsideration> considerations;
    public bool interruptable = false;
    public bool done = false;

    public UtilityAIAction()
    {
        name = "New Utility AI Action";
        action = new ExposedDelegate();
        considerations = new List<UtilityAIConsideration>();
    }

    public UtilityAIAction(UtilityAIAction action)
    {
        this.name = action.name;
        this.enabled = action.enabled;
        this.action = new ExposedDelegate(action.action);
        this.considerations = new List<UtilityAIConsideration>();
        foreach(UtilityAIConsideration consideration in action.considerations)
        {
            this.considerations.Add(new UtilityAIConsideration(consideration));
        }
    }


    public float CalculateScore(MonoBehaviour context)
    {
        float score = 1f;
        //For each of the action's considerations, calculate its score, and combine it with the total score:
        foreach(UtilityAIConsideration consideration in considerations)
        {
            if(consideration.enabled)
            {
                score = score * consideration.CalculateScore(consideration.considerationInput.GetInput(context));
                //If the score hits zero, there is no chance of it ever changing from 0, so return 0:
                if (score == 0)
                {
                    return 0;
                }
            }
        }
        //Apply the Compensation Factor:
        float originalScore = score;
        float modificationFactor = 1 - (1 / considerations.Count);
        float makeUpValue = (1 - originalScore) * modificationFactor;
        score = originalScore + (makeUpValue * originalScore);
        //Return the Score:
        return score;
    }

    public void EnableConsideration(string considerationName)
    {
        foreach(UtilityAIConsideration consideration in considerations)
        {
            if(consideration.name == considerationName)
            {
                consideration.enabled = true;
                return;
            }
        }
    }

    public void DisableConsideration(string considerationName)
    {
        foreach (UtilityAIConsideration consideration in considerations)
        {
            if(consideration.name == considerationName)
            {
                consideration.enabled = false;
                return;
            }
        }
    }

    public void DoAction()
    {
        action.Invoke();
    }
}
