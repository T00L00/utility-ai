using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    UtilityAIAgent agent;

    void Start()
    {
        agent = GetComponent<UtilityAIAgent>();
    }

    void Update()
    {
        UtilityAIAction bestAction = agent.GetBestAction();
        if(bestAction != null)
        {
            print(gameObject.name + " - " + bestAction.name);
        }
    }

    public void Heal(int test1, int test2)
    {
        print(test1 + " _ ");
    }
}
