using Lean.Gui;
using Lean.Transition;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class AppearOnHover : MonoBehaviour, IStateReactor
{
    private CanvasGroup canvasGroup;
    private float defaultAlpha;

    // Start is called before the first frame update
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        defaultAlpha = canvasGroup.alpha;
        canvasGroup.alpha = 0f;
    }

    public void Highlight(bool enabled)
    {
        if (enabled)
        {
            canvasGroup.alphaTransition(defaultAlpha, 0.1f);
        } else
        {
            canvasGroup.alphaTransition(0f, 0.1f);
        }
    }

    public void Focus(bool enabled)
    {
       
    }

    public void Select(bool enabled)
    {
       
    }

    public void Clear()
    {
        canvasGroup.alphaTransition(0f, 0.1f);
    }
}
