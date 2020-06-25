using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(ResponseCurve))]
public class ResponseCurvePropertyDrawer : PropertyDrawer
{
    private bool initDone;
    Material mat;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!initDone)
        {
            initDone = true;
            mat = new Material(Shader.Find("Hidden/Internal-Colored"));
        }

        ResponseCurve responseCurve = EditorExtensions.GetTargetObjectOfProperty(property) as ResponseCurve;
        EditorGUIUtility.labelWidth = 0f;
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        //create a rect to hold the graph in.
        Rect rect = EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        float size = rect.width < rect.height ? rect.width : rect.height;
        rect.width = size;
        rect.height = size;
        if (Event.current.type == EventType.Repaint)
        {
            GUI.BeginClip(rect);
            GL.PushMatrix();
            GL.Clear(true, false, Color.black);
            mat.SetPass(0);
            //Draw the background:
            GL.Begin(GL.QUADS);
            GL.Color(Color.black);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(rect.width, 0, 0);
            GL.Vertex3(rect.width, rect.height, 0);
            GL.Vertex3(0, rect.height, 0);
            GL.End();
            //Draw the grid:
            GL.Begin(GL.LINES);
            int count = (int)(rect.width / 10) + 20;
            float stepBackground = rect.width / 50;
            for(int i = 0; i < count; i++)
            {
                Color lineColor = i % 5 == 0 ? new Color(0.5f, 0.5f, 0.5f) : new Color(0.2f, 0.2f, 0.2f);
                GL.Color(lineColor);
                float x = i * stepBackground;
                //Draw vertical lines:
                if (x >= 0 && x < rect.width)
                {
                    GL.Vertex3(x, 0, 0);
                    GL.Vertex3(x, rect.height, 0);
                }
                //Draw horizontal lines:
                if (i < rect.height / stepBackground)
                {
                    GL.Vertex3(0, i * stepBackground, 0);
                    GL.Vertex3(rect.width, i * stepBackground, 0);
                }
            }
            //Draw the graph:
            GL.Color(Color.green);
            float step = 1 / size;
            for (float k = 0; k < 1; k += step)
            {
                GL.Vertex3(k * rect.width, rect.height - ResponseCurve.Evaluate(k, responseCurve) * rect.height, 0);
                GL.Vertex3((k + step) * rect.width, rect.height - ResponseCurve.Evaluate(k + step, responseCurve) * rect.height, 0);
            }
            GL.End();
            GL.PopMatrix();
            GUI.EndClip();
        }
       
        EditorGUILayout.EndHorizontal();
        EditorGUI.EndProperty();
    }
}
