using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UtilityAIConsideration : ScriptableObject
{
    public new string name;
    public bool enabled = true;
    public UtilityAIConsiderationInput considerationInput;
    public ResponseCurve responseCurve;
    
    public UtilityAIConsideration()
    {
        name = "New Utility AI Consideration";
        responseCurve = new ResponseCurve(CurveType.Linear, 1, 1, 0, 0);
    }

    public float CalculateScore(float input)
    {
        return ResponseCurve.Evaluate(input, responseCurve); 
    }

    
}
