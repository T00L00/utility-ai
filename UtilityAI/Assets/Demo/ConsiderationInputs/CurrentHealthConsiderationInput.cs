using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentHealthConsiderationInput : UtilityAIConsiderationInput
{
    override public float GetInput(MonoBehaviour context)
    {
        return 1f;
    }
}
