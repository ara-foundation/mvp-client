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
            Debug.LogWarning("No active tab");
            return;
        }

        if (from.CompareTag(ACTProjects.Instance.targetsTag) && enabled)
        {
            var dataGameObjectBehaviour = from.GetComponent<DataGameObjectBehaviour>();
            Debug.Log($"Target selected is null? {dataGameObjectBehaviour.DataGameObject == null}");
            activeTab.SelectionBehaviour.AddToSelection(dataGameObjectBehaviour.gameObject);
        } else
        {
            Debug.Log("Target NOT selected");

            activeTab.SelectionBehaviour.ClearSelection();
        }
    }
}
