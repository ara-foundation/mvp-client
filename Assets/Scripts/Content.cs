using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Content : MonoBehaviour
{
    public bool testMode = false;

    public void Clear()
    {
        if (testMode)
        {
            return;
        }
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        };
    }

    public GameObject Add(GameObject child)
    {
        var res = Instantiate(child, transform);
        return res;
    }

    public T Add<T>(GameObject child) where T : Component
    {
        var res = Add(child);
        return res.GetComponent<T>();
    }
}
