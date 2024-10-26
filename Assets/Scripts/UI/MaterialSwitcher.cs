using Lean.Gui;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// using UnityEngine.EventSystems;

[RequireComponent(typeof(MeshRenderer))]
public class MaterialSwitcher : MonoBehaviour, IStateReactor
{
    private MeshRenderer meshRenderer;
    public Material FocusMaterial;
    public Material HighlightMaterial;
    public Material SelectMaterial;
    private Material DefaultMaterial;
    private Material prevMaterial;

    // Start is called before the first frame update
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        DefaultMaterial = meshRenderer.material;
    }

    public void Highlight(bool enabled)
    {
        if (meshRenderer.material == FocusMaterial || meshRenderer.material == SelectMaterial)
        {
            return;
        }

        if (enabled)
        {
            prevMaterial = meshRenderer.material;
            meshRenderer.material = HighlightMaterial;
        } else
        {
            meshRenderer.material = DefaultMaterial;
        }
    }

    public void Focus(bool enabled)
    {
        if (!enabled)
        {
            meshRenderer.material = prevMaterial;
            return;
        }
        prevMaterial = meshRenderer.material;
        meshRenderer.material = FocusMaterial;
    }

    public void Select(bool enabled)
    {
        if (meshRenderer.material == FocusMaterial)
        {
            return;
        }
        if (!enabled)
        {
            meshRenderer.material = prevMaterial;
        } else
        {
            prevMaterial = meshRenderer.material;
            meshRenderer.material = SelectMaterial;

        }
    }

    public void Clear()
    {
        prevMaterial = DefaultMaterial;
        meshRenderer.material = DefaultMaterial;
    }
}
