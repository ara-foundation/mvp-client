using Lean.Gui;
using NBitcoin.Protocol;
using Rundo.RuntimeEditor.Behaviours.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

/// <summary>
/// Decentralized I/O System Data
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(CanvasGroup))]
public class DIOSObjectDraggable : MonoBehaviour
{
    CanvasGroup canvasGroup;

    public delegate void SelectCallback();
    public event SelectCallback OnSelected;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Disable();
    }

    public void Disable()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void Enable()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnSelect()
    {
        Debug.Log("Selectable clicked");
        OnSelected?.Invoke();
    }
}
