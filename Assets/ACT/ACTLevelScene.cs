using Lean.Gui;
using Rundo.RuntimeEditor.Behaviours.UI;
using Rundo.RuntimeEditor.Behaviours;
using Rundo.RuntimeEditor.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rundo.RuntimeEditor.Commands;
using Rundo;

public class ACTLevelScene : EditorBaseBehaviour
{
    private List<ACTPart_interface> Parts = new ();

    [SerializeField] private LeanWindow PrimitivesWindow;
    [SerializeField] private LeanWindow LineWindow;
    [SerializeField] private GameObject LinePrefab;
    private static ACTLevelScene _instance;

    private bool _lineMode = false;
    private DataGameObject _editingLineData = null;
    private ACTPart_line _editingLinePart = null;

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
                UnsetLineMode(cancel: true);
            }
        }
    }

    private void UnsetLineMode(bool cancel)
    {
        _lineMode = false;
        if (cancel)
        {
            DestroyDataGameObjectCommand.Process(DataScene, _editingLineData);
        }
        SetPartsLineMode(false);
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

    public void AddPart(ACTPart_interface part)
    {
        var partType = part.GetType();
        if (partType == typeof(ACTPart_line))
        {
            Debug.Log("Add part of line instance");

            if (_lineMode)
            {
                _editingLinePart = part as ACTPart_line;
            }
        } else
        {
            Debug.Log("Add part of non-line instance");
            if (!Parts.Contains(part))
            {
                Parts.Add(part);
            }
        }
    }

    public ACTPart_interface GetPart(string objectId)
    {
        foreach (var part in Parts)
        {
            if (part.ObjectId().Equals(objectId))
            {
                return part;
            }
        }

        return null;
    }

    public void RemovePart(ACTPart_interface part)
    {
        var partType = part.GetType();
        if (partType == typeof(ACTPart_line))
        {
            if (_lineMode)
            {
                _editingLinePart = null;
            }
        } else
        {
            if (Parts.Contains(part))
            {
                Parts.Remove(part);
            }
        }
    }

    public void SetPartsInteractiveMode(bool on)
    {
        foreach (ACTPart part in Parts)
        {
            part.Interactive(on);
        }
    }

    private void SetPartsLineMode(bool on)
    {
        foreach (ACTPart part in Parts)
        {
            part.SetLineMode(on);
        }
    }

    /// <summary>
    /// Changes the scene to the line placing mode.
    /// This method is called from Sensever Dialogue
    /// </summary>
    public void SetLineMode()
    {
        _lineMode = true;
        SetPartsLineMode(true);
        InstantiateLine();
    }

    private void InstantiateLine()
    {
        if (LinePrefab.TryGetComponent<PrefabIdBehaviour>(out var prefabIdBehaviour))
        {
            _editingLineData = DataScene.InstantiateDataGameObjectFromPrefab(prefabIdBehaviour);

            _editingLineData.GetComponent<DataGameObjectBehaviour>().DataComponentPrefab.OverridePrefabComponent = true;
            _editingLineData.GetComponent<DataTransformBehaviour>().DataComponentPrefab.OverridePrefabComponent = true;
            
            // Sends to the Rundo to add the line.
            // The instantiniated game object will call the ACTPart_interface.Activate method.
            // The ACTPart_interface.Activate calls ACTLevelScene.AddPart back.
            CreateDataGameObjectCommand.Process(DataScene, _editingLineData, DataScene);
        }
    }

    public void OnLinePointSelect(ACTPart part)
    {
        Debug.Log("ACTLevel scene set a part as a point");
        if (_editingLinePart != null)
        {
            if (_editingLinePart.OnSetPoint(part))
            {
                UnsetLineMode(cancel: false);
            }
        }
    }
}
