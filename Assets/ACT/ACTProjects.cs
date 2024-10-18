using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACTProjects : MonoBehaviour
{
    public ActivityGroup ActivityGroup;
    private static ACTProjects _instance;

    public static ACTProjects Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ACTProjects>();
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
