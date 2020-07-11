using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExposedDelegate
{
    [SerializeField]
    public List<DelegateEntry> delegateEntries;

    public ExposedDelegate()
    {
        delegateEntries = new List<DelegateEntry>();
    }

    public ExposedDelegate(ExposedDelegate exposedDelegate)
    {
        this.delegateEntries = new List<DelegateEntry>();
        foreach (DelegateEntry delegateEntry in exposedDelegate.delegateEntries)
        {
            this.delegateEntries.Add(new DelegateEntry(delegateEntry));
        }
    }

    public void Invoke()
    {
        foreach(DelegateEntry delegateEntry in delegateEntries)
        {
            delegateEntry.Invoke();
        }
    }
}
