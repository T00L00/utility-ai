using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class ResponseCurveEditor : VisualElement
{
    UtilityAIConsiderationEditor utilityAIConsiderationEditor;
    ResponseCurve responseCurve;

    public ResponseCurveEditor(UtilityAIConsiderationEditor utilityAIConsiderationEditor, ResponseCurve responseCurve)
    {
        this.utilityAIConsiderationEditor = utilityAIConsiderationEditor;
        this.responseCurve = responseCurve;

        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UtilityAI/Scripts/Editor/Response Curve Editor/ResponseCurveEditor.uxml");
        visualTree.CloneTree(this);

        StyleSheet stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UtilityAI/Scripts/Editor/Response Curve Editor/ResponseCurveEditor.uss");
        this.styleSheets.Add(stylesheet);

        this.AddToClassList("responseCurveEditor");

        EnumField curveTypeField = this.Query<EnumField>("curveType").First();
        curveTypeField.Init(CurveType.Linear);
        curveTypeField.value = responseCurve.curveType;
        curveTypeField.RegisterCallback<ChangeEvent<Enum>>(
            e =>
            {
                responseCurve.curveType = (CurveType)e.newValue;
                utilityAIConsiderationEditor.UpdateResponseCurve();
            }
        );

        FloatField xShiftField = this.Query<FloatField>("xShift").First();
        xShiftField.value = responseCurve.xShift;
        xShiftField.RegisterCallback<ChangeEvent<float>>(
            e =>
            {
                responseCurve.xShift = (float)e.newValue;
                utilityAIConsiderationEditor.UpdateResponseCurve();
            }
        );

        FloatField yShiftField = this.Query<FloatField>("yShift").First();
        yShiftField.value = responseCurve.yShift;
        yShiftField.RegisterCallback<ChangeEvent<float>>(
            e =>
            {
                responseCurve.yShift = (float)e.newValue;
                utilityAIConsiderationEditor.UpdateResponseCurve();
            }
        );

        FloatField slopeField = this.Query<FloatField>("slope").First();
        slopeField.value = responseCurve.slope;
        slopeField.RegisterCallback<ChangeEvent<float>>(
            e =>
            {
                responseCurve.slope = (float)e.newValue;
                utilityAIConsiderationEditor.UpdateResponseCurve();
            }
        );

        FloatField exponentialField = this.Query<FloatField>("exponential").First();
        exponentialField.value = responseCurve.exponential;
        exponentialField.RegisterCallback<ChangeEvent<float>>(
            e =>
            {
                responseCurve.exponential = (float)e.newValue;
                utilityAIConsiderationEditor.UpdateResponseCurve();
            }
        );

        Box box = new Box()
        {
            style =
            {
                flexGrow = 1,
                marginTop = 5,
                marginBottom = 5,
                marginLeft = 5,
                marginRight = 5,
                height = 300,
            }
        };
        this.Add(box);
        
        VisualElement meshContainer = new VisualElement() { style = { flexGrow = 1, } };
        box.Add(meshContainer);
        meshContainer.generateVisualContent += DrawGraph;
    }

    void DrawGraph(MeshGenerationContext context)
    {
        float step = 1 / context.visualElement.contentRect.width;
        List<Vector3> points = new List<Vector3>();
        for (float i = 0; i <= 1; i += step)
        {
            points.Add(new Vector3(i, ResponseCurve.Evaluate(i, responseCurve), Vertex.nearZ));
        }
        float thickness = 2f;
        Color color = Color.red;

        List<Vertex> vertices = new List<Vertex>();
        List<ushort> indices = new List<ushort>();

        for (int i = 0; i < points.Count - 1; i++)
        {
            var pointA = points[i];
            var pointB = points[i + 1];

            pointA.x = pointA.x * context.visualElement.contentRect.width;
            pointA.y = context.visualElement.contentRect.height - pointA.y * context.visualElement.contentRect.height;

            pointB.x = pointB.x * context.visualElement.contentRect.width;
            pointB.y = context.visualElement.contentRect.height - pointB.y * context.visualElement.contentRect.height;

            float angle = Mathf.Atan2(pointB.y - pointA.y, pointB.x - pointA.x);
            float offsetX = thickness / 2 * Mathf.Sin(angle);
            float offsetY = thickness / 2 * Mathf.Cos(angle);

            vertices.Add(new Vertex()
            {
                position = new Vector3(pointA.x + offsetX, pointA.y - offsetY, Vertex.nearZ),
                tint = color
            });
            vertices.Add(new Vertex()
            {
                position = new Vector3(pointB.x + offsetX, pointB.y - offsetY, Vertex.nearZ),
                tint = color
            });
            vertices.Add(new Vertex()
            {
                position = new Vector3(pointB.x - offsetX, pointB.y + offsetY, Vertex.nearZ),
                tint = color
            });
            vertices.Add(new Vertex()
            {
                position = new Vector3(pointB.x - offsetX, pointB.y + offsetY, Vertex.nearZ),
                tint = color
            });
            vertices.Add(new Vertex()
            {
                position = new Vector3(pointA.x - offsetX, pointA.y + offsetY, Vertex.nearZ),
                tint = color
            });
            vertices.Add(new Vertex()
            {
                position = new Vector3(pointA.x + offsetX, pointA.y - offsetY, Vertex.nearZ),
                tint = color
            });

            ushort indexOffset(int value) => (ushort)(value + (i * 6));
            indices.Add(indexOffset(0));
            indices.Add(indexOffset(1));
            indices.Add(indexOffset(2));
            indices.Add(indexOffset(3));
            indices.Add(indexOffset(4));
            indices.Add(indexOffset(5));
        }

        var mesh = context.Allocate(vertices.Count, indices.Count);
        mesh.SetAllVertices(vertices.ToArray());
        mesh.SetAllIndices(indices.ToArray());
    }
}