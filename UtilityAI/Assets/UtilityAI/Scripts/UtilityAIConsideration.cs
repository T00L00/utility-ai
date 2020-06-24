using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ResponseCurve {Linear, Sin, Binary};

[System.Serializable]
public class UtilityAIConsideration
{
    public string name;
    public bool enabled = true;
    public UtilityAIConsiderationInput considerationInput;
    public ResponseCurve responseCurve;
    public float slope = 1f;
    public float exponential = 1f;
    public float xShift = 0f;
    public float yShift = 0f;

    public static float CalculateScore(float input, ResponseCurve responseCurve, float slope, float exponential, float xShift, float yShift)
    {
        float score = 0;
        switch(responseCurve)
        {
            case ResponseCurve.Linear:
                score = CalculateLinear(input, slope, exponential, xShift, yShift);
                break;
            case ResponseCurve.Sin:
                score = CalculateSin(input, slope, exponential, xShift, yShift);
                break;
            case ResponseCurve.Binary:
                score = CalculateBinary(input, slope, xShift, yShift);
                break;
            default:
                break;
        }
        return Mathf.Clamp(score, 0, 1);
    }

    public static float CalculateLinear(float input, float slope, float exponential, float xShift, float yShift)
    {
        return slope * Mathf.Pow((input - xShift), exponential) + yShift;
    }
    public static float CalculateSin(float input, float slope, float exponential, float xShift, float yShift)
    {
        return Mathf.Pow(slope * Mathf.Sin(input + xShift), exponential) + yShift;
    }
    public static float CalculateBinary(float input, float slope, float xShift, float yShift)
    {
        if(slope >= 0)
        {
            return input <= xShift ? 0 + yShift : 1 + yShift;
        }
        else
        {
            return input > xShift ? 0 + yShift : 1 + yShift;
        }
    }
}
