using System;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class DelegateEntry
{
    [SerializeField]
    protected GameObject targetGO;
    public GameObject TargetGO { get { return targetGO; } set { targetGO = value; } }

    [SerializeField]
    protected UnityEngine.Object target;
    public UnityEngine.Object Target { get { return target; } set { target = value; } }

    [SerializeField]
    protected SerializableMethodInfo methodInfo;
    public MethodInfo Method {
        get
        {
            if (methodInfo != null)
            {
                return methodInfo.methodInfo;
            }
            else
            {
                return null;
            }
        }
        set 
        { 
            methodInfo = new SerializableMethodInfo(value); 
        } 
    }

    [SerializeField]
    protected SerializableObject[] parameters;
    public SerializableObject[] Parameters { get { return parameters; } set { parameters = value; } }

    public DelegateEntry()
    {
    }

    public DelegateEntry(DelegateEntry delegateEntry)
    {
        this.Target = delegateEntry.Target;
        this.Method = delegateEntry.Method;
        this.Parameters = delegateEntry.Parameters;
    }

    public void SetMethod(Type targetType, string methodName)
    {
        methodInfo = new SerializableMethodInfo(targetType.GetMethod(methodName));
        ParameterInfo[] pInfos = methodInfo.methodInfo.GetParameters();
        parameters = new SerializableObject[pInfos.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            parameters[i] = new SerializableObject(pInfos[i].ParameterType);
        }
        target = targetGO.GetComponent(targetType);
    }

    public void Invoke()
    {
        object[] deserializedParameters = new object[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            deserializedParameters[i] = parameters[i].obj;
        }
        methodInfo.methodInfo.Invoke(target, deserializedParameters);
    }
}
