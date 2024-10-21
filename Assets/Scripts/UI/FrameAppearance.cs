using Lean.Gui;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// using UnityEngine.EventSystems;

public class FrameAppearance: MonoBehaviour, IStateReactor
{
    [Header("Mouse Handler")]
    public bool ChangeColorOnFocus = true;
    public bool DisableFocus = false;

    private Color FrameDefaultColor = Color.white;
    public Color FrameFocusColor = Color.blue;
    public Color FrameHighlightColor = Color.blue;
    public Color FrameSelectColor = Color.white;
    public Image Frame = null;
    public RawImage RawFrame = null;

    private void Awake()
    {
        if (Frame == null && RawFrame == null)
        {
            Debug.LogError("Frame not set in " + transform.name + " of " + transform.parent.name);
            return;
        }

        if (Frame != null)
        {
            FrameDefaultColor = Frame.color;
        } else
        {
            FrameDefaultColor = RawFrame.color;
        }
    }

    public void Clear()
    {
        if (Frame == null && RawFrame == null)
        {
            return;
        }
        if (Frame != null)
        {
            Frame.color = FrameDefaultColor;
        } else
        {
            RawFrame.color = FrameDefaultColor;
        }
    }
   
    public void Select(bool enabled)
    {
        if (Frame == null && RawFrame == null)
        {
            return;
        }
        if (enabled)
        {
            if (Frame != null)
                Frame.color = FrameSelectColor;
            else
                RawFrame.color = FrameSelectColor;
        }
        else
        {
            if (Frame != null)
            {
                Frame.color = FrameDefaultColor;
            } else
            {
                RawFrame.color = FrameDefaultColor;
            }

        }
    }

    public void Highlight(bool enabled)
    {
        if (Frame == null && RawFrame == null)
        {
            return;
        }
        if (enabled)
        {
            if (Frame != null)
            {
                Frame.color = FrameHighlightColor;
            } else
            {
                RawFrame.color = FrameHighlightColor;
            }
        } else {
            if (Frame != null)
            {
                Frame.color = FrameDefaultColor;
            } else
            {
                RawFrame.color = FrameDefaultColor;
            }
        }
    }

    public void Focus(bool enabled)
    {
        if (Frame == null || RawFrame == null || DisableFocus)
        {
            return;
        }
        if (enabled)
        {
            if (ChangeColorOnFocus)
            {
                if (Frame != null)
                {
                    Frame.color = FrameFocusColor;
                }
                else
                {
                    RawFrame.color = FrameFocusColor;
                }
            }
        }
        else
        {
            if (ChangeColorOnFocus)
            {
                if (Frame != null)
                {
                    Frame.color = FrameDefaultColor;
                }
                else
                {
                    RawFrame.color = FrameDefaultColor;
                }
            }
        }
    }
}
