using Lean.Gui;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextAppearance: MonoBehaviour, IStateReactor
{
    private Color TextDefaultColor = Color.white;
    public Color TextFocusColor = Color.blue;
    public Color TextHighlightColor = Color.blue;
    public Color TextSelectColor = Color.white;
    public Text Text = null;
    public TMPro.TMP_Text TextPro = null;

    private Color getTextColor()
    {
        if (TextPro != null)
        {
            return TextPro.color;
        }
        return Text.color;
    }

    private void setTextColor(Color color)
    {
        if (TextPro != null)
        {
            TextPro.color = color;
            return;
        }
        Text.color = color;
    }

    private bool IsTextSet()
    {
        return (Text != null || TextPro != null);
    }

    private void Awake()
    {
        if (!IsTextSet())
        {
            Debug.LogError("Text not set for " + transform.name + " of " + transform.parent.name);
            return;
        }

        TextDefaultColor = getTextColor();
    }

    public void Highlight(bool enabled)
    {
        if (!IsTextSet())
        {
            return;
        }
        if (enabled)
        {
            setTextColor(TextHighlightColor);
        } else {
            setTextColor(TextDefaultColor); 
        }
    }

    public void Select(bool enabled)
    {
        if (!IsTextSet())
        {
            return;
        }
        if (enabled)
        {
            setTextColor(TextSelectColor);
        }
        else
        {
            setTextColor(TextDefaultColor);
        }
    }

    public void Focus(bool enabled)
    {
        return;
    }

    public void Clear()
    {
        setTextColor(TextDefaultColor);
    }
}
