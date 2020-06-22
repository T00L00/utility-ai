using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UtilityAIAgent : MonoBehaviour
{
    public List<UtilityAIAction> actions;

    public UtilityAIAction GetBestAction(List<UtilityAIAction> UtilityAIActions)
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
        //Else return the best action:
        else
        {
            return bestActions[0];
        }
    }

    public void EnableAction(string actionName)
    {
        for(int action = 0; action < actions.Count; action++)
        {
            if(actions[action].name == actionName)
            {
                actions[action].enabled = true;
                return;
            }
        }
    }

    public void DisableAction(string actionName)
    {
        for(int action = 0; action < actions.Count; action++)
        {
            if(actions[action].name == actionName)
            {
                actions[action].enabled = false;
                return;
            }
        }
    }
}
