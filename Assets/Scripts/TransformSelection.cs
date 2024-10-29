using Ara.RuntimeEditor;
using RTS_Cam;
using Rundo.RuntimeEditor.Behaviours;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class TransformSelection : MonoBehaviour
{
    public void SelectTarget(Transform from, bool enabled)
    {
        var activeTab = AraRuntimeEditor_manager.Instance.GetActiveTab();
        if (activeTab == null)
        {
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
                activeTab.SelectionBehaviour.AddToSelection(dataGameObjectBehaviour.gameObject);
            }
        } else
        {
            activeTab.SelectionBehaviour.ClearSelection();
        }
    }
}
