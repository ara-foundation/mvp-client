using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACTLevelScene : MonoBehaviour
{
    private List<ACTPart> Parts = new ();

    [SerializeField] private LeanWindow PrimitivesWindow;
    [SerializeField] private LeanWindow LineWindow;
    private static ACTLevelScene _instance;

    private bool _lineMode = false;

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

    private void Awake()
    {
        PrimitivesWindow.gameObject.SetActive(false);
        LineWindow.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Don't destroy it since, it references to all other Objects
        DontDestroyOnLoad(this);       
    }

    void Update ()
    {
        if (_lineMode)
        {
            if (Input.GetMouseButtonDown(1))
            {
                LineModeParts(false);
                Debug.Log("Exit from the line mode. Todo(remove the spline controller)");
                _lineMode = false;
            }
        }
    }

    // Update is called once per frame
    public void OnPrimitivesWindowSelect(bool selected)
    {
        PrimitivesWindow.gameObject.SetActive(selected);
        PrimitivesWindow.Set(selected);
    }

    public void OnLineWindowSelect(bool selected)
    {
        LineWindow.gameObject.SetActive(selected);
        LineWindow.Set(selected);
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

    private void LineModeParts(bool on)
    {
        foreach (ACTPart part in Parts)
        {
            part.SetLineMode(on);
        }
    }

    public void SetLineMode()
    {
        _lineMode = true;
        Debug.Log("Set the line mode. Todo(create a spline controller, perhaps from prefab?)");
        LineModeParts(true);
    }
}
