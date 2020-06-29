using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SerializableObject
{
    public SerializableObject(Type type)
    {
        if (type == typeof(int))
        {
            obj = 0;
        }
        else if (type == typeof(float))
        {
            obj = 0.0f;
        }
        else if (type == typeof(bool))
        {
            obj = false;
        }
        else if (type == typeof(string))
        {
            obj = "";
        }
        else if (type == typeof(Vector3))
        {
            obj = new Vector3();
        }
        else if (type == typeof(Vector2))
        {
            obj = new Vector2();
        }
        else if (type == typeof(Bounds))
        {
            obj = new Bounds();
        }
        else if (type == typeof(Rect))
        {
            obj = new Rect();
        }
        else if (type == typeof(Color))
        {
            obj = new Color();
        }
        else if (type == typeof(UnityEngine.Object))
        {
            obj = new UnityEngine.Object();
        }
        else if (type.IsEnum)
        {
            obj = Enum.Parse(type, type.GetEnumNames()[0]);
        }
    }

    public SerializableType typeUsed;
    public object obj
    {
        set
        {
            if (value != null)
            {
                typeUsed = new SerializableType(value.GetType());
                if (value is int)
                {
                    _intValue = (int)value;
                }
                else if (value is float)
                {
                    _floatValue = (float)value;
                }
                else if (value is bool)
                {
                    _boolValue = (bool)value;
                }
                else if (value is string)
                {
                    _stringValue = (string)value;
                }
                else if (value is Vector3)
                {
                    _vector3Value = (Vector3)value;
                }
                else if (value is Vector2)
                {
                    _vector2Value = (Vector2)value;
                }
                else if (value is Bounds)
                {
                    _boundsValue = (Bounds)value;
                }
                else if (value is Rect)
                {
                    _rectValue = (Rect)value;
                }
                else if (value is Color)
                {
                    _colorValue = (Color)value;
                }
                else if (typeUsed.type.IsEnum)
                {
                    _enumOptions = Enum.GetNames(typeUsed.type);
                    _enumIndex = _enumOptions.ToList().IndexOf(value.ToString());
                }
                else if (value is UnityEngine.Object)
                {
                    _objectValue = (UnityEngine.Object)value;
                }
            }
        }
        get
        {
            if (typeUsed.type != null)
            {
                if (typeUsed.type == typeof(int))
                {
                    return _intValue;
                }
                else if (typeUsed.type == typeof(float))
                {
                    return _floatValue;
                }
                else if (typeUsed.type == typeof(bool))
                {
                    return _boolValue;
                }
                else if (typeUsed.type == typeof(string))
                {
                    return _stringValue;
                }
                else if (typeUsed.type == typeof(Vector3))
                {
                    return _vector3Value;
                }
                else if (typeUsed.type == typeof(Vector2))
                {
                    return _vector2Value;
                }
                else if (typeUsed.type == typeof(Bounds))
                {
                    return _boundsValue;
                }
                else if (typeUsed.type == typeof(Rect))
                {
                    return _rectValue;
                }
                else if (typeUsed.type == typeof(Color))
                {
                    return _colorValue;
                }
                else if (typeUsed.type.IsEnum)
                {
                    return Enum.Parse(typeUsed.type, _enumOptions[_enumIndex]);
                }
                else if (typeof(UnityEngine.Object).IsAssignableFrom(typeUsed.type))
                {
                    return _objectValue;
                }
            }
            return null;
        }
    }

    [SerializeField]
    private int _intValue;
    [SerializeField]
    private float _floatValue;
    [SerializeField]
    private bool _boolValue;
    [SerializeField]
    private string _stringValue = string.Empty;
    [SerializeField]
    private Vector3 _vector3Value;
    [SerializeField]
    private Vector2 _vector2Value;
    [SerializeField]
    private Bounds _boundsValue;
    [SerializeField]
    private Rect _rectValue;
    [SerializeField]
    private Color _colorValue;
    [SerializeField]
    private int _enumIndex;
    [SerializeField]
    private string[] _enumOptions;
    [SerializeField]
    private UnityEngine.Object _objectValue;
}

