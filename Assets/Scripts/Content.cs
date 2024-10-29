using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Content : MonoBehaviour
{
    public void Clear()
    {
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
}
