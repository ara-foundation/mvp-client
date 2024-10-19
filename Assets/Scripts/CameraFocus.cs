using RTS_Cam;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    [SerializeField]
    private RTS_Camera cam;

    public string targetsTag; // objects must have this element

    public void SelectTarget(Transform from, bool enabled)
    {
        if (from.CompareTag(targetsTag) && enabled)
        {
            cam.SetTarget(from);
        } else
        {
            cam.ResetTarget();
        }
    }
}
