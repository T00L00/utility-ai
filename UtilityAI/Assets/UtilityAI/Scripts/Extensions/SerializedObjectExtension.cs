using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public class EditorExtensions
{
    /// <Summary> 
    /// Takes a serialized property a returns its target object.
    /// </Summary> 
    /// <param name="prop"> A serialised property. </param>
    /// <returns> The object that is targetted by the serialised property. </returns>
    public static object GetTargetObjectOfProperty(SerializedProperty prop)
    {
        //If the property is null, return:
        if (prop == null)
        {
            return null;
        }
        //Get the property's path, and its serialised object's target:
        string path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        //Split the property path into its elements, and loop over each element:
        string[] pathElements = path.Split('.');
        foreach (string element in pathElements)
        {
            //If the element references an array/list object:
            if (element.Contains("["))
            {
                //Get the index of the target object in the array:
                string elementName = element.Substring(0, element.IndexOf("["));
                int index = System.Convert.ToInt32((element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", "")));
                //Get the next object from the array:
                obj = GetValue_Imp(obj, elementName, index);
            }
            else
            {
                //Get the next object:
                obj = GetValue_Imp(obj, element);
            }
        }
        //return the object that is targetted by the serialised property:
        return obj;
    }

    /// <Summary> 
    /// Takes an object and the next property path element, returning the object at the next property path element.
    /// </Summary> 
    /// <param name="source"> The current loaded object. </param>
    /// <param name="name"> The next element of the property path. </param>
    /// <returns> The object at the next property path element. </returns>
    private static object GetValue_Imp(object source, string name)
    {
        //If the source object is null, return:
        if (source == null)
        {
            return null;
        }
        //Get the type of source object:
        System.Type type = source.GetType();
        while (type != null)
        {
            //Get the field for the type, and if its not null return its value:
            FieldInfo f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f != null)
            {
                return f.GetValue(source);
            }
            //Get the property for the type, and if its not null return its value:
            PropertyInfo p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p != null)
            {
                return p.GetValue(source, null);
            }
            //If both the property and field for the current type were null, get the type that the current type inherits from:
            type = type.BaseType;
        }
        //If no suitable fields/properties were found, return null:
        return null;
    }

    /// <Summary> 
    /// Takes an object that is a list/array and the next property path element, returning the object at the next property path element.
    /// </Summary> 
    /// <param name="source"> The current loaded object. </param>
    /// <param name="name"> The next element of the property path. </param>
    /// <returns> The object at the next property path element. </returns>
    private static object GetValue_Imp(object source, string name, int index)
    {
        //Get the array/list as an IEnumerable, if not found return null:
        System.Collections.IEnumerable enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
        if (enumerable == null)
        {
            return null;
        }
        //Loop over the IEnumerable until the index of the target object is met:
        IEnumerator enm = enumerable.GetEnumerator();
        for (int i = 0; i <= index; i++)
        {
            //If the IEnumerable is not long enough return:
            if (!enm.MoveNext())
            {
                return null;
            }
        }
        //Return the target object:
        return enm.Current;
    }
}
