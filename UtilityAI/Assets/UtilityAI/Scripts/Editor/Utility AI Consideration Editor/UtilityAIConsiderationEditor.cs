using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class UtilityAIConsiderationEditor : VisualElement
{
    UtilityAIConsideration consideration;
    UtilityAIActionEditor utilityAIActionEditor;

    Foldout considerationContainerFoldout;
    VisualElement responseCurveContainer;

    public UtilityAIConsiderationEditor(UtilityAIActionEditor utilityAIActionEditor, UtilityAIConsideration consideration)
    {
        this.consideration = consideration;
        this.utilityAIActionEditor = utilityAIActionEditor;

        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UtilityAI/Scripts/Editor/Utility AI Consideration Editor/UtilityAIConsiderationEditor.uxml");
        visualTree.CloneTree(this);

        StyleSheet stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UtilityAI/Scripts/Editor/Utility AI Consideration Editor/UtilityAIConsiderationEditor.uss");
        this.styleSheets.Add(stylesheet);

        this.AddToClassList("utilityAIActionEditor");

        considerationContainerFoldout = this.Query<Foldout>("consideration").First();
        considerationContainerFoldout.text = consideration.name;

        responseCurveContainer = this.Query<VisualElement>("responseCurve").First();

        TextField nameField = this.Query<TextField>("considerationName").First();
        nameField.value = consideration.name;
        nameField.RegisterCallback<ChangeEvent<string>>(
            e =>
            {
                consideration.name = (string)e.newValue;
                considerationContainerFoldout.text = (string)e.newValue;
            }
        );

        Toggle enabledField = this.Query<Toggle>("considerationEnabled").First();
        enabledField.value = consideration.enabled;
        enabledField.RegisterCallback<ChangeEvent<bool>>(
            e =>
            {
                consideration.enabled = (bool)e.newValue;
            }
        );

        ObjectField inputField = this.Query<ObjectField>("considerationInput").First();
        inputField.objectType = typeof(UtilityAIConsiderationInput);
        inputField.value = consideration.considerationInput;
        inputField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(
            e =>
            {
                consideration.considerationInput = (UtilityAIConsiderationInput)e.newValue;
            }
        );

        UpdateResponseCurve();
    }

    public void UpdateResponseCurve()
    {
        responseCurveContainer.Clear();
        ResponseCurveEditor responseCurveEditor = new ResponseCurveEditor(this, consideration.responseCurve);
        responseCurveContainer.Add(responseCurveEditor);
    }
}