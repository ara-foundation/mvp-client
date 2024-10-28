using Lean.Gui;
using Lean.Transition;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// using UnityEngine.EventSystems;

public class Rotator : MonoBehaviour
{
    public float delay = 0.2f;
    public Vector3 rotateAngle = Vector3.zero;
    public float duration = 0.2f;
    [Tooltip("Leave empty to make this object as a target")]
    [SerializeField] private Transform Target;
    private bool rotated = false;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (!rotated)
        {
            StartCoroutine(Rotate());
        }
    }

    IEnumerator Rotate()
    {
        rotated = true;
        yield return new WaitForSeconds(delay);

        if (Target != null )
        {
            Target.eulerAnglesTransform(rotateAngle, duration, LeanEase.Smooth);
        } else
        {
            transform.eulerAnglesTransform(rotateAngle, duration, LeanEase.Smooth);
        }
    }
}
