using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum StateMode
{
    None = 0,               // Default
    Selected,               // Double Click
    Highlighted,            // On mouse enter
    Unhighlight,            // On mouse leave
    Focused,                // Single Click
    Transitioning           // During animation
}

/// <summary>
/// ActivityState handles whether the given object is highlighted,
/// selected or focused.
/// 
/// <list type="number">
///     <listheader>The activity could be changed by</listheader> 
///     <item>MouseHandler</item> 
///     <item>KeyboardHandler</item>
///     <item>From Scene</item>
/// </list>
/// </summary>
public class ActivityState : MonoBehaviour
{
    private StateMode mode = StateMode.None;
    public StateMode Mode { get { return mode; } }

    public ActivityGroup ActivityGroup;
    [SerializeField]
    public UnityEngine.Object[] reactors;

    private bool lateChange;
    private StateMode lateChangeMode;

    private void Awake()
    {
        lateChange = false;
        if (ActivityGroup != null)
        {
            ActivityGroup.SetActivityState(this);
        }
    }

    public void SetActivityGroup(ActivityGroup activityGroup)
    {
        ActivityGroup = activityGroup;
        if (ActivityGroup != null)
        {
            ActivityGroup.SetActivityState(this);
        }
    }

    private void Update()
    {
        if (lateChange)
        {
            ChangeMode(lateChangeMode);
            lateChange = false;
        }
        
    }

    public void LateChangeMode(StateMode to)
    {
        lateChange = true;
        lateChangeMode = to;
    }

    public void ChangeMode(StateMode to)
    {
        bool change = false;
        StateMode updatedTo = to;

        switch (to)
        {
            case StateMode.None:
                change = true;
                break;
            case StateMode.Unhighlight:
                if (mode == StateMode.Highlighted)
                {
                    change = true;
                }
                break;
            case StateMode.Highlighted:
                if (mode == StateMode.Unhighlight || mode == StateMode.None)
                {
                    change = true;
                }
                break;
            case StateMode.Selected:
                if (mode != StateMode.Transitioning && mode != StateMode.Focused)
                {
                    change = true;
                }
                break;
            case StateMode.Focused:
                if (mode != StateMode.Transitioning)
                {
                    change = true;
                }
                break;
            
            default:
                break;
        }
        if (!change)
        {
            return;
        }

        foreach (var reactorObj in reactors)
        {
            var reactor = reactorObj.GetComponent<IStateReactor>();

            switch (to)
            {
                case StateMode.Unhighlight:
                    reactor.Highlight(false);
                    break;
                case StateMode.Highlighted:
                    reactor.Highlight(true);
                    break;
                case StateMode.Selected:
                    if (mode != StateMode.Transitioning && mode != StateMode.Focused)
                    {
                        if (mode == StateMode.Highlighted)
                        {
                            reactor.Highlight(false);
                        }
                        if (mode == StateMode.Selected)
                        {
                            reactor.Select(false);
                            reactor.Highlight(true);
                            updatedTo = StateMode.Highlighted;
                        }
                        else
                        {
                            reactor.Select(true);
                        }
                    }
                    break;
                case StateMode.Focused:
                    if (mode != StateMode.Transitioning)
                    {
                        if (mode == StateMode.Highlighted)
                        {
                            reactor.Highlight(false);
                        }
                        if (mode == StateMode.Selected)
                        {
                            reactor.Select(false);
                        }
                        if (mode == StateMode.Focused)
                        {
                            reactor.Focus(false);
                            reactor.Highlight(true);
                            updatedTo = StateMode.Highlighted;
                        }
                        else
                        {
                            reactor.Focus(true);
                        }
                    }
                    break;
                default:
                    if (mode == StateMode.Highlighted)
                    {
                        reactor.Highlight(false);
                    }
                    if (mode == StateMode.Selected)
                    {
                        reactor.Select(false);
                    }
                    if (mode == StateMode.Focused)
                    {
                        reactor.Focus(false);
                    }

                    reactor.Clear();
                    break;
            }
        }

        if (ActivityGroup != null)
        {
            ActivityGroup.ChangeActivity(this, updatedTo);
        }

        mode = updatedTo;
    }

    public void Select(bool enabled)
    {
        if (enabled)
        {
            ChangeMode(StateMode.Selected);
        } else
        {
            ChangeMode(StateMode.None);
        }
    }

    private void OnDestroy()
    {
        if (ActivityGroup != null)
        {
            ActivityGroup.DeleteActivityState(this);
        }
    }
}