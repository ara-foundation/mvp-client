using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACTSession : MonoBehaviour
{
    private static ACTSession _instance;

    public ActWithProjectAndPlan Project = null;
    public int Level = 0;

    public static ACTSession Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ACTSession>();
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetFirstLevel(ActWithProjectAndPlan _project)
    {
        Project = _project;
        Level = 1;
    }

    public void Clear()
    {
        Destroy(this.gameObject);
    }

}
