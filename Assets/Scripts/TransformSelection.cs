using Ara.RuntimeEditor;
using Lean.Gui;
using RTS_Cam;
using Rundo.RuntimeEditor.Behaviours;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class TransformSelection : MonoBehaviour
{
    [SerializeField] private LeanWindow TransformUI;

    private void Start()
    {
        TransformUI.TurnOff();
    }

    public void SelectTarget(Transform from, bool enabled)
    {
        var activeTab = AraRuntimeEditor_manager.Instance.GetActiveTab();
        if (activeTab == null)
        {
            TransformUI.TurnOff();
            return;
        }

        if (from.CompareTag(ACTProjects.TargetTag) && enabled)
        {
            var part = from.GetComponent<ACTPart>();
            if (part.Mode != ACTPart.ModeInScene.Interactive)
            {
                return;
            }
            var notOrphaned = from.TryGetComponent<DataGameObjectBehaviour>(out var dataGameObjectBehaviour);
            if (notOrphaned)
            {
                TransformUI.TurnOn();
                activeTab.SelectionBehaviour.AddToSelection(dataGameObjectBehaviour.gameObject);
            }
        } else
        {
            TransformUI.TurnOff();
            activeTab.SelectionBehaviour.ClearSelection();
        }
    }
}
