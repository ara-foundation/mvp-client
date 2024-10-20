using RTS_Cam;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    [SerializeField]
    private RTS_Camera cam;

    public void SelectTarget(Transform from, bool enabled)
    {
        if (from.CompareTag(ACTProjects.Instance.targetsTag) && enabled)
        {
            cam.SetTarget(from);
        } else
        {
            cam.ResetTarget();
        }
    }
}
