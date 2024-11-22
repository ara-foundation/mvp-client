using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Body : MonoBehaviour
{
    [SerializeField]
    private RectTransform[] Fixed = { };
    [SerializeField]
    private RectTransform[] Dynamic = { };

    private RectTransform _transform;


    private void OnEnable()
    {
        _transform = GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        if (Fixed.Length > 0 || Dynamic.Length > 0)
        {
            UpdateLists();
            UpdateRectTransform();
        }
    }

    void UpdateRectTransform()
    {
        if (Dynamic.Length == 0 || _transform == null)
        {
            return;
        }

        Vector2 newSize = _transform.sizeDelta;
        newSize.y = 0;
        foreach (var dynamic in Dynamic) {
            newSize.y += dynamic.sizeDelta.y;
        }
        _transform.sizeDelta = newSize;
    }

    void UpdateLists()
    {
        int dynamicLength = gameObject.transform.childCount - Fixed.Length;
        if (dynamicLength != Dynamic.Length)
        {
            Dynamic = new RectTransform[dynamicLength];

            for (int i = 0, dI = 0; i < gameObject.transform.childCount; i++)
            {
                var child = gameObject.transform.GetChild(i).GetComponent<RectTransform>();
                if (child == null) {  continue; }

                bool isFixed = false;
                for (int j = 0; j < Fixed.Length; j++)
                {
                    if (Fixed[j] == child)
                    {
                        isFixed = true;
                        break;
                    }
                }

                if (!isFixed)
                {
                    Dynamic[dI] = child;

                    dI++;
                }
            }
        }
    }
}
