using Lean.Gui;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// using UnityEngine.EventSystems;

[RequireComponent(typeof(Outline))]
public class MeshOutliner : MonoBehaviour, IStateReactor
{
    private Outline outline;
    public Color FocusColor = Color.blue;
    public Color HighlightColor = Color.blue;
    public Color SelectColor = Color.yellow;
    public GameObject meshBody;
    private Color prevColor = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void Highlight(bool enabled)
    {
        if (outline.enabled && (outline.OutlineColor == FocusColor || outline.OutlineColor == SelectColor))
        {
            return;
        }

        outline.enabled = enabled;
        outline.OutlineColor = HighlightColor;
    }

    public void Focus(bool enabled)
    {
        outline.enabled = enabled;
        outline.OutlineColor = FocusColor;
    }

    public void Select(bool enabled)
    {
        if (outline.enabled && outline.OutlineColor == FocusColor)
        {
            return;
        }

        outline.enabled = enabled;
        outline.OutlineColor = SelectColor;
    }

    public void Clear()
    {
        outline.enabled = false;
    }
}
