using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACTLevelScene : MonoBehaviour
{
    [SerializeField] private LeanWindow PrimitivesWindow;
    private static ACTLevelScene _instance;

    public static ACTLevelScene Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ACTLevelScene>();
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

    // Update is called once per frame
    public void OnPrimitivesWindowSelect(bool selected)
    {
        PrimitivesWindow.Set(selected);
    }
}
