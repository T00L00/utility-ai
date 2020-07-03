using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UtilityAIAction : ScriptableObject
{
    public new string name;
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


    public float CalculateScore(MonoBehaviour context)
    {
        float score = 1f;
        //For each of the action's considerations, calculate its score, and combine it with the total score:
        foreach(UtilityAIConsideration consideration in considerations)
        {
            if(consideration.enabled)
            {
                score = score * consideration.CalculateScore(consideration.considerationInput.GetInput(context));
            }
            //If the score hits zero, there is no chance of it ever changing from 0, so return 0:
            if(score == 0)
            {
                return 0;
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
        for(int consideration = 0; consideration < considerations.Count; consideration++)
        {
            if(considerations[consideration].name == considerationName)
            {
                considerations[consideration].enabled = true;
                return;
            }
        }
    }

    public void DisableConsideration(string considerationName)
    {
        for(int consideration = 0; consideration < considerations.Count; consideration++)
        {
            if(considerations[consideration].name == considerationName)
            {
                considerations[consideration].enabled = false;
                return;
            }
        }
    }

    public void DoAction()
    {
        action.Invoke();
    }
}
