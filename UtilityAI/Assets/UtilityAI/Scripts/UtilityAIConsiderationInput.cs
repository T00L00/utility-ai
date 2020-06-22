using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class UtilityAIConsiderationInput : MonoBehaviour
{
    /// <Summary> 
    /// Overridable function that can be used to get an input for a consideration.
    /// </Summary> 
    /// <param name="context"> A MonoBehaviour script that will provide context for a consideration's input. </param>
    /// <returns> A floating point number representing the input for the consideration's response curve (should be normalised between 0 and 1) </returns>
    abstract public float GetInput(MonoBehaviour context);
}
