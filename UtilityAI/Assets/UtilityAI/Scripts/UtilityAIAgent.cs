using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UtilityAIAgent : MonoBehaviour
{
    public UtilityAIActionSet actionSet;

    public List<UtilityAIAction> actions = new List<UtilityAIAction>();

    private void Start()
    {
        MakeActionsSetUnique();
    }

    public UtilityAIAction GetBestAction()
    {
        float topScore = 0.0f;
        List<UtilityAIAction> bestActions = new List<UtilityAIAction>();
        //Loop over each enabled action and calculate its score:
        foreach(UtilityAIAction action in actions)
        {
            if(action.enabled == true)
            {
                float score = action.CalculateScore(this);
                //If the actions score is greater than the current best, make this action the only current best action:
                if(score > topScore)
                {
                    topScore = score;
                    bestActions.Clear();
                    bestActions.Add(action);
                }
                //Else if the actions score is equal to the current best, append it to the list of best actions:
                else if(score == topScore)
                {
                    bestActions.Add(action);
                }
            }
        }
        //If there are multiple current best actions, return a random one:
        if(bestActions.Count > 1)
        {
            return bestActions[Random.Range(0, bestActions.Count)];
        }
        //Else if there is one best action return it:
        else if(bestActions.Count > 0)
        {
            return bestActions[0];
        }
        //Else return null:
        else
        {
            return null;
        }
    }

    public void EnableAction(string actionName)
    {
        foreach(UtilityAIAction action in actions)
        {
            if(action.name == actionName)
            {
                action.enabled = true;
                return;
            }
        }
    }

    public void DisableAction(string actionName)
    {
        foreach(UtilityAIAction action in actions)
        { 
            if(action.name == actionName)
            {
                action.enabled = false;
                return;
            }
        }
    }

    public void MakeActionsSetUnique()
    {
        if (actionSet != null)
        {
            List<UtilityAIAction> sharedActions = actionSet.actions;
            List<UtilityAIAction> uniqueActions = new List<UtilityAIAction>();
            foreach(UtilityAIAction sharedAction in sharedActions)
            {
                uniqueActions.Add(new UtilityAIAction(sharedAction));
            }
            actions = uniqueActions;
            actionSet = null;
        }
    }
}
