using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UtilityAIConsideration
{
    public string name;
    public bool enabled = true;
    public UtilityAIConsiderationInput considerationInput;
    public ResponseCurve responseCurve;
    
    public float CalculateScore(float input)
    {
        return ResponseCurve.Evaluate(input, responseCurve); 
    }

    
}
