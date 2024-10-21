using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACTProjects : MonoBehaviour
{
    public ActivityGroup ActivityGroup;
    private static ACTProjects _instance;

    public Camera Camera;
    public string targetsTag; // objects must have this element

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
        // Don't destroy it since, it references to all other Objects
        DontDestroyOnLoad(this);       
    }
}
