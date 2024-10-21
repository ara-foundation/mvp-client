using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACTLevelScene : MonoBehaviour
{
    private List<ACTPart> Parts = new ();

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

    public void AddPart(ACTPart part)
    {
        if (!Parts.Contains(part))
        {
            Parts.Add(part);
        }
    }

    public void RemovePart(ACTPart part)
    {
        if (Parts.Contains(part))
        {
            Parts.Remove(part);
        }
    }

    public void InteractiveParts(bool on)
    {
        foreach (ACTPart part in Parts)
        {
            part.Interactive(on);
        }
    }
}
