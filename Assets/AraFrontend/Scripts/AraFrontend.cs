using Ara.RuntimeEditor;
using Lean.Gui;
using PieChart.ViitorCloud;
using Rundo.RuntimeEditor.Behaviours.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AraFrontend : MonoBehaviour
{
    public Camera MainCamera;
    private static AraFrontend _instance;
    RaycastHit hit;
    public LayerMask IgnoreMask;

    private List<GameObject> currentRaysastResults = new();
    [SerializeField] private InputSelectionWindow SelectionWindow;
    [SerializeField] private ActivityGroup MenuButtons;

    [Serializable]
    public enum ActionType
    {
        Input = 0,
        Output = 1,
        Timer = 2
    }

    bool IsGOInMask(GameObject a, LayerMask m)
    {
        return ((1 << a.layer) & m) != 0;
    }

    private List<GameObject> SelectedItems { get { return currentRaysastResults;  } }

    private List<ProjectItemMetaData> SelectedData()
    {
        var res = new List<ProjectItemMetaData>();

        foreach (var go in currentRaysastResults)
        {
            var dataType = DIOSData.GameObjectDataType(go);
            var data = new ProjectItemMetaData { GameObject = go, DiosType = dataType, };
            res.Add(data);
        }

        return res;
    }

    public static AraFrontend Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AraFrontend>();
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MainCamera = Camera.main;
        SelectionWindow.TurnOff(ifOn: false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // Make sure that it wasn't clicked on the Button
            CatchMousePoint();
        }
    }

    public bool CatchMousePoint()
    {
        return CatchMousePoint(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool CatchMousePoint(List<RaycastResult> eventSystemRaysastResults)
    {
        if (eventSystemRaysastResults.Count == 0)
        {
            return false;
        }
        var replaceCurrent = true;
        if (eventSystemRaysastResults.Count == currentRaysastResults.Count)
        {
            for (var i = 0; i < currentRaysastResults.Count; i++)
            {
                if (eventSystemRaysastResults[i].gameObject.GetInstanceID() == currentRaysastResults[i].GetInstanceID())
                {
                    replaceCurrent = false;
                    continue;
                }
                replaceCurrent = true;
                break;
            }    
        }

        if (replaceCurrent)
        {
            // no ignored mask
            for (var i = 0; i < eventSystemRaysastResults.Count; i++)
            {
                var detectedObj = eventSystemRaysastResults[i].gameObject;
                var isIgnored = IsGOInMask(detectedObj, IgnoreMask);
                if (isIgnored)
                {
                    return false;
                }
            }

            /*var obj = eventSystemRaysastResults[0].gameObject;
            if (obj.TryGetComponent<Button>(out _) || obj.TryGetComponent<LeanButton>(out _))
            {
                Debug.Log($"Clicked on the button {obj.name}");
                return false;
            }
            if (obj.transform.parent != null)
            {
                var parent = obj.transform.parent;
                if (parent.TryGetComponent<Button>(out _) || parent.TryGetComponent<LeanButton>(out _))
                {
                    Debug.Log($"The {obj.name} parent {parent.name} is a button. this is likely a text or icon of button");
                    return false;
                }
            }*/

            var count = eventSystemRaysastResults.Count;
            Debug.Log($"{System.DateTime.Now} {count} objects under:");
            for (var i = 0; i < count; i++)
            {
                var diosData = DIOSData.GameObjectDataType(eventSystemRaysastResults[i].gameObject);
                if (diosData.Count == 1 && diosData[0] == DIOSData.Type.Zero)
                {
                    Debug.Log($"{eventSystemRaysastResults[i].gameObject.name} is not a dios data skip");
                } else if (diosData.Count == 1 && diosData[0] == DIOSData.Type.NoData)
                {
                    Debug.Log($"{eventSystemRaysastResults[i].gameObject.name} has no data?");
                } else
                {
                    var go = eventSystemRaysastResults[i].gameObject;
                    currentRaysastResults.Add(go);
                    var types = "";
                    for (var j = 0; j < diosData.Count; j++)
                    {
                        types += $"{j}/{diosData.Count}={diosData[j].ToString()} ";
                    }
                    Debug.Log($"\t{i+1}/{count}: object under mouse {go.name} as dios {types} types {Environment.NewLine}");
                }
            }
            return true;
        } else
        {
            currentRaysastResults.Clear();
        }

        return false;
    }

    public void OnItemSelect(ProjectItemMetaData metaData, ActionType actionType)
    {
        Notification.Instance.Show($"Item {metaData.GameObject.name} of {metaData.DiosType[0].ToString()} was selected for {actionType}");
        ClearSelection();
        MenuButtons.UnSelectAll();
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    public void OnInput(bool select)
    {
        Debug.Log($"OnInput {select}");
        if (!select)
        {
            SelectionWindow.TurnOff(ifOn: true);
            return;
        }
        if (currentRaysastResults == null || currentRaysastResults.Count == 0) {
            Debug.Log("No objects were selected, show the input list");
        } else
        {
            SelectionWindow.TurnOn(selected: SelectedData(), ActionType.Input);
        }
    }

    public void ClearSelection()
    {
        currentRaysastResults.Clear();
    }

    public void OnOutput(bool select)
    {
        if (!select)
        {
            SelectionWindow.TurnOff(ifOn: true);
            return;
        }
        if (currentRaysastResults == null || currentRaysastResults.Count == 0)
        {
            Debug.Log("No objects were selected, show the input list");
        }
        else
        {
            SelectionWindow.TurnOn(selected: SelectedData(), ActionType.Output);
        }
        Debug.Log("Clicked on output");
    }

    public void OnTimer(bool select)
    {
        if (!select)
        {
            SelectionWindow.TurnOff(ifOn: true);
            return;
        }
        if (currentRaysastResults == null || currentRaysastResults.Count == 0)
        {
            Debug.Log("No objects were selected, show the input list");
        }
        else
        {
            SelectionWindow.TurnOn(selected: SelectedData(), ActionType.Timer);
        }
    }
}
