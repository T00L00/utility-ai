using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UtilityAIConsideration
{
    public string name;
    public bool enabled;
    public AnimationCurve curve; 
    public UtilityAIConsiderationInput considerationInput;

    public float CalculateScore(MonoBehaviour context)
    {
        float score = curve.Evaluate(considerationInput.GetInput(context));
        return Mathf.Clamp(score, 0, 1);
    }
}
