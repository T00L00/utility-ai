using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExposedDelegate
{
    [SerializeField]
    public List<DelegateEntry> delegateEntries = new List<DelegateEntry>();

    public void Invoke()
    {
        foreach(DelegateEntry delegateEntry in delegateEntries)
        {
            delegateEntry.Invoke();
        }
    }
}
