using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum CurveType {Linear, Sin, Binary};


[System.Serializable]
public class ResponseCurve
{
    public CurveType curveType;
    public float slope = 1f;
    public float exponential = 1f;
    public float xShift = 0f;
    public float yShift = 0f;

    public ResponseCurve(ResponseCurve responseCurve)
    {
        curveType = responseCurve.curveType;
        slope = responseCurve.slope;
        exponential = responseCurve.exponential;
        xShift = responseCurve.xShift;
        yShift = responseCurve.yShift;
    }

    public ResponseCurve(CurveType newCurveType, float newSlope, float newExponential, float newXShift, float newYShift)
    {
        curveType = newCurveType;
        slope = newSlope;
        exponential = newExponential;
        xShift = newXShift;
        yShift = newYShift;
    }

    public static float Evaluate(float input, ResponseCurve responseCurve)
    {
        float score = 0;
        switch(responseCurve.curveType)
        {
            case CurveType.Linear:
                score = CalculateLinear(input, responseCurve.slope, responseCurve.exponential, responseCurve.xShift, responseCurve.yShift);
                break;
            case CurveType.Sin:
                score = CalculateSin(input, responseCurve.slope, responseCurve.exponential, responseCurve.xShift, responseCurve.yShift);
                break;
            case CurveType.Binary:
                score = CalculateBinary(input, responseCurve.slope, responseCurve.xShift, responseCurve.yShift);
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
