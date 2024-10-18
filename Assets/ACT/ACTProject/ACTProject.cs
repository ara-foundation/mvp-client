using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACTProject : MonoBehaviour
{
    [SerializeField]
    public ActivityState ActivityState;

    // Start is called before the first frame update
    void Start()
    {
        ActivityState.SetActivityGroup(ACTProjects.Instance.ActivityGroup);
    }

}
