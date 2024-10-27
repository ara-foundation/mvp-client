using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Make a callback methods when activity state changed
/// </summary>
public class EventInvoker : MonoBehaviour, IStateReactor
{
    bool selected = false;
    bool focused = false;
    bool highlighted = false;

    public UnityEngine.Events.UnityEvent<bool> OnSelect;
    public UnityEngine.Events.UnityEvent<bool> OnFocus;
    public UnityEngine.Events.UnityEvent<bool> OnHighlight;

    public void Clear()
    {
        if (selected)
        {
            selected = false;
            OnSelect?.Invoke(selected);
        }
        if (focused)
        {
            focused = false;
            OnFocus?.Invoke(focused);
        }
        if (highlighted)
        {
            highlighted = false;
            OnHighlight?.Invoke(highlighted);
        }
    }

    public void Focus(bool enabled)
    {
        if (enabled)
        {
            focused = true;
        }
        else
        {
            focused = false;
        }
        OnFocus?.Invoke(focused);
    }

    public void Highlight(bool enabled)
    {
        if (enabled)
        {
            highlighted = true;
        }
        else
        {
            highlighted = false;
        }
        OnHighlight?.Invoke(highlighted);
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
        OnSelect?.Invoke(selected);
    }
}
