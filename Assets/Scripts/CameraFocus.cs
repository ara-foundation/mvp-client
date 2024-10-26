using RTS_Cam;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    [SerializeField]
    private RTS_Camera cam;

    private static CameraFocus _instance;

    public static CameraFocus Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CameraFocus>();
            }
            return _instance;
        }
    }

    /// <summary>
    /// Call it for any object in the scene. For example for the input fields.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="enabled"></param>
    public void SelectTargetThrough(Transform from, bool enabled)
    {
        if (from.CompareTag(ACTProjects.Instance.targetsTag) && enabled)
        {
            cam.SetTarget(from);
        }
        else
        {
            cam.ResetTarget();
        }
    }

    /// <summary>
    /// Call it only for the interactive act parts in the scene
    /// </summary>
    /// <param name="from"></param>
    /// <param name="enabled"></param>
    public void SelectTarget(Transform from, bool enabled)
    {
        if (from.CompareTag(ACTProjects.Instance.targetsTag) && enabled)
        {
            var part = from.GetComponent<ACTPart>();
            if (part.Mode != ACTPart.ModeInScene.Interactive)
            {
                return;
            }
            cam.SetTarget(from);
        } else
        {
            cam.ResetTarget();
        }
    }
}
