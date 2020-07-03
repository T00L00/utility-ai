using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class DelegateEntryEditor : VisualElement
{
    ExposedDelegateEditor exposedDelegateEditor;
    DelegateEntry delegateEntry;

    Foldout delegateEntryFoldout;

    public DelegateEntryEditor(ExposedDelegateEditor exposedDelegateEditor, DelegateEntry delegateEntry)
    {
        this.exposedDelegateEditor = exposedDelegateEditor;
        this.delegateEntry = delegateEntry;

        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UtilityAI/Scripts/Editor/Delegate Entry Editor/DelegateEntryEditor.uxml");
        visualTree.CloneTree(this);

        StyleSheet stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UtilityAI/Scripts/Editor/Delegate Entry Editor/DelegateEntryEditor.uss");
        this.styleSheets.Add(stylesheet);

        this.AddToClassList("delegateEntryEditor");

        delegateEntryFoldout = this.Query<Foldout>("delegateEntry");
        if(delegateEntry.TargetGO != null)
        {
            delegateEntryFoldout.text += delegateEntry.TargetGO.name;
            if (delegateEntry.Target != null)
            {
                delegateEntryFoldout.text += "(" + delegateEntry.Target.GetType() + ") ";
                if (delegateEntry.Method != null)
                {
                    delegateEntryFoldout.text += delegateEntry.Method.Name;
                }
            }
            delegateEntryFoldout.text += ": ";
        }
        else
        {
            delegateEntryFoldout.text = "New Delegate Entry:";
        }
        

        delegateEntryFoldout.Query<Toggle>().First().AddToClassList("delegateEntryFoldout");

        Button moveUpButton = this.Query<Button>("moveUpButton").First();
        moveUpButton.BringToFront();
        moveUpButton.clickable.clicked += MoveEntryUp;

        Button moveDownButton = this.Query<Button>("moveDownButton").First();
        moveDownButton.BringToFront();
        moveDownButton.clickable.clicked += MoveEntryDown;

        Button deleteButton = this.Query<Button>("deleteButton").First();
        deleteButton.BringToFront();
        deleteButton.clickable.clicked += DeleteEntry;

        ObjectField targetGOField = this.Query<ObjectField>("targetGOField").First();
        targetGOField.objectType = typeof(GameObject);
        targetGOField.value = delegateEntry.TargetGO;
        targetGOField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(
            e =>
            {
                delegateEntry.TargetGO = (GameObject)e.newValue;
                exposedDelegateEditor.UpdateDelegateEntries();
            }
        );
        if (delegateEntry.TargetGO != null)
        {
            List<Component> components = delegateEntry.TargetGO.GetComponents<Component>().ToList();
            PopupField<Component> targetComponentField = new PopupField<Component>("Component: ", components, components.Contains(delegateEntry.Target) ? components.IndexOf((Component) delegateEntry.Target) : 0);
            delegateEntry.Target = targetComponentField.value;
            targetComponentField.RegisterCallback<ChangeEvent<Component>>(
                e =>
                {
                    delegateEntry.Target = (Component)e.newValue;
                    exposedDelegateEditor.UpdateDelegateEntries();
                }
            );
            delegateEntryFoldout.Add(targetComponentField);
            if(delegateEntry.Target != null)
            {
                Type selectedComponentType = delegateEntry.Target.GetType();
                BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
                //Get a list of methods attached to the component, and create a dropdown menu:
                List<MethodInfo> methods = selectedComponentType.GetMethods(flags).ToList();
                PopupField<MethodInfo> targetMethodField = new PopupField<MethodInfo>("Method: ", methods, methods.Contains(delegateEntry.Method) ? methods.IndexOf(delegateEntry.Method) : 0);
                if(delegateEntry.Method == null || delegateEntry.Method.Name != targetMethodField.value.Name)
                {
                    delegateEntry.SetMethod(selectedComponentType, targetMethodField.value.Name);
                }
                targetMethodField.RegisterCallback<ChangeEvent<MethodInfo>>(
                    e =>
                    {
                        delegateEntry.SetMethod(selectedComponentType, e.newValue.Name);
                        exposedDelegateEditor.UpdateDelegateEntries();
                    }
                );
                delegateEntryFoldout.Add(targetMethodField);
                if(delegateEntry.Method != null && delegateEntry.Parameters.Length > 0)
                {
                    Foldout parametersFoldout = new Foldout();
                    parametersFoldout.text = "Parameters: ";

                    foreach(SerializableObject parameter in delegateEntry.Parameters)
                    {
                        if (parameter.obj is int)
                        {
                            IntegerField parameterField = new IntegerField();
                            parameterField.value = (int)parameter.obj;
                            parameterField.RegisterCallback<ChangeEvent<int>>(
                                e =>
                                {
                                    parameter.obj = (int)e.newValue;
                                }
                            );
                            parametersFoldout.Add(parameterField);
                        }
                        else if (parameter.obj is float)
                        {
                            FloatField parameterField = new FloatField();
                            parameterField.value = (float)parameter.obj;
                            parameterField.RegisterCallback<ChangeEvent<float>>(
                                e =>
                                {
                                    parameter.obj = (float)e.newValue;
                                }
                            );
                            parametersFoldout.Add(parameterField);
                        }
                        else if (parameter.obj is bool)
                        {
                            Toggle parameterField = new Toggle();
                            parameterField.value = (bool)parameter.obj;
                            parameterField.RegisterCallback<ChangeEvent<bool>>(
                                e =>
                                {
                                    parameter.obj = (bool)e.newValue;
                                }
                            );
                            parametersFoldout.Add(parameterField);
                        }
                        else if (parameter.obj is string)
                        {
                            TextField parameterField = new TextField();
                            parameterField.value = (string)parameter.obj;
                            parameterField.RegisterCallback<ChangeEvent<string>>(
                                e =>
                                {
                                    parameter.obj = (string)e.newValue;
                                }
                            );
                            parametersFoldout.Add(parameterField);
                        }
                        else if(parameter.obj is Vector3)
                        {
                            Vector3Field parameterField = new Vector3Field();
                            parameterField.value = (Vector3) parameter.obj;
                            parameterField.RegisterCallback<ChangeEvent<Vector3>>(
                                e =>
                                {
                                    parameter.obj = (Vector3)e.newValue;
                                }
                            );
                            parametersFoldout.Add(parameterField);
                        }
                        else if (parameter.obj is Vector2)
                        {
                            Vector2Field parameterField = new Vector2Field();
                            parameterField.value = (Vector2)parameter.obj;
                            parameterField.RegisterCallback<ChangeEvent<Vector2>>(
                                e =>
                                {
                                    parameter.obj = (Vector2)e.newValue;
                                }
                            );
                            parametersFoldout.Add(parameterField);
                        }
                        else if (parameter.obj is Bounds)
                        {
                            BoundsField parameterField = new BoundsField();
                            parameterField.value = (Bounds)parameter.obj;
                            parameterField.RegisterCallback<ChangeEvent<Bounds>>(
                                e =>
                                {
                                    parameter.obj = (Bounds)e.newValue;
                                }
                            );
                            parametersFoldout.Add(parameterField);
                        }
                        else if (parameter.obj is Rect)
                        {
                            RectField parameterField = new RectField();
                            parameterField.value = (Rect)parameter.obj;
                            parameterField.RegisterCallback<ChangeEvent<Rect>>(
                                e =>
                                {
                                    parameter.obj = (Rect)e.newValue;
                                }
                            );
                            parametersFoldout.Add(parameterField);
                        }
                        else if (parameter.obj is Color)
                        {
                            ColorField parameterField = new ColorField();
                            parameterField.value = (Color)parameter.obj;
                            parameterField.RegisterCallback<ChangeEvent<Color>>(
                                e =>
                                {
                                    parameter.obj = (Color)e.newValue;
                                }
                            );
                            parametersFoldout.Add(parameterField);
                        }
                        else if (parameter.obj is UnityEngine.Object)
                        {
                            ObjectField parameterField = new ObjectField();
                            parameterField.value = (UnityEngine.Object)parameter.obj;
                            parameterField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(
                                e =>
                                {
                                    parameter.obj = (UnityEngine.Object)e.newValue;
                                }
                            );
                            parametersFoldout.Add(parameterField);
                        }
                    }

                    delegateEntryFoldout.Add(parametersFoldout);
                }
            }
        }
        //Add parameters for the selected method:
        Foldout parameterFoldout = this.Query<Foldout>("parameters");
    }

    private void MoveEntryUp()
    {
        int index = exposedDelegateEditor.exposedDelegate.delegateEntries.IndexOf(delegateEntry);
        exposedDelegateEditor.utilityAIActionEditor.utilityAIAgentEditor.SwapItemsInCollection(exposedDelegateEditor.utilityAIActionEditor.action.action.delegateEntries, index, index - 1);
    }

    private void MoveEntryDown()
    {
        int index = exposedDelegateEditor.exposedDelegate.delegateEntries.IndexOf(delegateEntry);
        exposedDelegateEditor.utilityAIActionEditor.utilityAIAgentEditor.SwapItemsInCollection(exposedDelegateEditor.utilityAIActionEditor.action.action.delegateEntries, index, index + 1);
    }

    private void DeleteEntry()
    {
        if (EditorUtility.DisplayDialog("Delete Delegate Entry", "Are you sure you want to remove this delegate entry from the action?", "Delete", "Cancel"))
        {
           exposedDelegateEditor.utilityAIActionEditor.utilityAIAgentEditor.RemoveItemFromCollection(exposedDelegateEditor.utilityAIActionEditor.action.action.delegateEntries, delegateEntry);
        }
    }
}