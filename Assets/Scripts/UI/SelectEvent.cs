using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectEvent : MonoBehaviour, IStateReactor
{
    bool selected = false;

    public UnityEngine.Events.UnityEvent<bool> OnSelect;

    public void Clear()
    {
        if (selected)
        {
            selected = false;
            OnSelect.Invoke(selected);
        }
    }

    public void Focus(bool enabled)
    {
    }

    public void Highlight(bool enabled)
    {
    }

    public void Select(bool enabled)
    {
        if (enabled)
        {
            selected = true;
        } else
        {
            selected = false;
        }
        OnSelect.Invoke(selected);
    }
}
