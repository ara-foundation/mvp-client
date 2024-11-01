using Dreamteck.Splines;
using Lean.Transition;
using Newtonsoft.Json;
using Rundo.RuntimeEditor.Behaviours;
using Rundo.RuntimeEditor.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static ACTPart_edit;

[Serializable]
public class SaveModelResult
{
    [SerializeField]
    public ACTPartModel part;
    public bool increasePartsAmount;
}

public class ACTPart : EditorBaseBehaviour, IStateReactor, ACTPart_interface
{
    [Serializable]
    public enum ModeInScene
    {
        Interactive, // Default mode
        View,   // During the object placeholder, make it view
        DrawLine, // For drawing the line change the select
        Edit, // Same as interactive but hides the transform handler
    }
    [HideInInspector]
    protected Canvas Canvas;
    [SerializeField]
    public ActivityState ActivityState;
    [HideInInspector]
    protected GameObject Menu;
    [SerializeField]
    protected MouseInput MouseInput;
    [HideInInspector] 
    protected Transform SplinePositionersContent;
    [SerializeField]
    protected List<Node> Connections = new();
    [SerializeField, HideInInspector]
    protected ACTPart_edit Edit;

    [Tooltip("For lines for now no controller used")]
    public bool EnableController;
    public ModeInScene Mode = ModeInScene.View;
    protected DataGameObjectId objectId;

    /// <summary>
    /// Orphan means its called manually without loading from the scripts.
    /// Orphan scripts will show the canvas.
    /// </summary>
    private bool orphaned = false;

    public Vector3 LinePointPosition()
    {
        return SplinePositionersContent.position;
    }

    /// <summary>
    /// The last spline positioner is set by the line setter via AddSplinePositioner.
    /// Therefore, call of this function is a reverse operation. To avoid misuse, makes sure its in draw line mode.
    /// </summary>
    public void DeleteLastSplinePositioner()
    {
        if (Mode != ModeInScene.DrawLine)
        {
            return;
        }

        if (Connections.Count > 0)
        {
            var lastIndex = Connections.Count - 1;
            var last = Connections[lastIndex];
            Destroy(last.gameObject);
            Connections.RemoveAt(lastIndex);
        }
    }

    public void ConnectToLine(SplineComputer spline, int positionIndex)
    {
        if (Mode != ModeInScene.DrawLine) {
            return;
        }
        var objToSpawn = new GameObject($"SplinePositioner {Connections.Count + 1}");
        objToSpawn.transform.parent = SplinePositionersContent;
        
        var connection = objToSpawn.AddComponent<Node>();
        connection.AddConnection(spline, positionIndex);
     
        objToSpawn.transform.SetLocalPositionAndRotation(Vector3.zero, objToSpawn.transform.localRotation);
    }

    // Start is called before the first frame update
    void Start()
    {
        var found = TryGetComponent<DataGameObjectBehaviour>(out _);
        orphaned = !found;
        if (orphaned)
        {
            Debug.LogWarning($"Orphaned {gameObject.name}! The orphaned act parts are not applied by the transform. Also make sure that changes are net send to the server");
        }

        if (Mode != ModeInScene.Interactive && Mode != ModeInScene.Edit)
        {
            if (orphaned)
            {
                Activate(DataGameObjectId.Create(Time.deltaTime.ToString()));
            }
        }
    }

    public string ObjectId()
    {
        return objectId.ToStringRawValue();
    }

    public void Activate(DataGameObjectId objectId)
    {
        if (EnableController)
        {
            if (ACTLevelScene.Instance != null)
            {
                var obj = Instantiate(ACTLevelScene.Instance.PartControllerPrefab, transform);
                var controller = obj.GetComponent<ACTPart_controller>();
                Canvas = controller.Canvas;
                Menu = controller.Menu;
                SplinePositionersContent = controller.SplinePositionerContent;

                Edit = obj.GetComponent<ACTPart_edit>();
                if (Edit != null)
                {
                    Edit.OnModelEdited += OnModelEdited;
                }
            }
        }

        this.objectId = objectId;
        Mode = ModeInScene.Interactive;
        ActivityState.SetActivityGroup(ACTLevelScene.Instance.ActivityGroup);
        Menu.SetActive(false);
        ACTLevelScene.Instance.AddPart(this);
        Canvas.worldCamera = ACTLevelScene.Instance.Camera;
        Canvas.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        if (Mode == ModeInScene.Interactive || Mode == ModeInScene.Edit)
        {
            ACTLevelScene.Instance.RemovePart(this);
            Menu.SetActive(false);
        }
    }

    public void Select(bool enabled)
    {
        if (Mode == ModeInScene.Interactive || Mode == ModeInScene.Edit)
        {
            Menu.SetActive(enabled);
        } else if (Mode == ModeInScene.DrawLine)
        {
            if (enabled)
            {
                ACTLevelScene.Instance.OnLinePointSelect(this);
            }
        }
    }

    public void Focus(bool enabled)
    {
    }

    public void Highlight(bool enabled)
    {
    }

    public void Clear()
    {
    }

    ////////////////////////////////////////////////////
    ///
    ///  Change the mode of part in the scene
    ///
    ////////////////////////////////////////////////////
    public void Interactive(bool on)
    {
        if ((on && Mode == ModeInScene.Interactive) || (!on && Mode == ModeInScene.View))
        {
            return;
        }
        if (!on)
        {
            ActivityState.ChangeMode(StateMode.None);
            Mode = ModeInScene.View;
        } else
        {
            Mode = ModeInScene.Interactive;
        }
        MouseInput.enabled = on;
    }

    public void SetEditMode(bool on)
    {
        if (!on)
        {
            ActivityState.ChangeMode(StateMode.None);
            Mode = ModeInScene.View;
        }
        else
        {
            Mode = ModeInScene.Edit;

            ActivityState.Select(true);
        }
        MouseInput.enabled = on;
    }

    public void EditProjectName(ProjectNameEditedDelegate onEdited)
    {
        if (Edit == null)
        {
            Notification.Instance.Show("ACTPart_edit not set. Are you on Line since lines dont have edit yet");
            return;
        }

        // Start editing the project name
        Edit.OnProjectNameEdited += onEdited;
        Edit.OnEditProjectName(true);
    }

    public void EditTechStack(TechStackEditedDelegate onEdited)
    {
        if (Edit == null)
        {
            Notification.Instance.Show("ACTPart_edit not set. Are you on Line since lines dont have edit yet");
            return;
        }

        Edit.OnTechStackEdited += onEdited;
        Edit.OnEditTechStack(true);
    }

    public void SetLineMode(bool on)
    {
        if ((on && Mode == ModeInScene.DrawLine) || (!on && Mode == ModeInScene.View))
        {
            return;
        }
        if (!on)
        {
            ActivityState.ChangeMode(StateMode.None);
            Mode = ModeInScene.View;
        }
        else
        {
            Mode = ModeInScene.DrawLine;
        }
        Debug.Log($"Set line mode {on} for {gameObject.name}");
        MouseInput.enabled = on;
    }

    /// <summary>
    /// Fetch the part parameters for the development id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private async Task<Tuple<bool, bool>> SaveModel(ACTPartModel model)
    {
        string body = JsonConvert.SerializeObject(model);
        string url = NetworkParams.AraActUrl + "/act/part";

        Tuple<long, string> res;
        try
        {
            res = await WebClient.Post(url, body);
        }
        catch (Exception ex)
        {
            Notification.Instance.Show($"Error to save model: client exception {ex.Message}");
            Debug.LogError(ex);
            return Tuple.Create(false, false);
        }
        if (res.Item1 != 200)
        {
            Notification.Instance.Show($"Error to save model: {res.Item2}");
            return Tuple.Create(false, false);
        }

        Notification.Instance.Show($"Part was saved successfully!");

        SaveModelResult result;
        try
        {
            result = JsonConvert.DeserializeObject<SaveModelResult>(res.Item2);
        }
        catch (Exception e)
        {
            Debug.LogError(e + " for " + res);
            return Tuple.Create(false, false);
        }
        return Tuple.Create(true, result.increasePartsAmount);
    }

    private async void OnModelEdited(ACTPartModel _model)
    {
        if (Mode == ModeInScene.Interactive)
        {
            var saveResult = await SaveModel(_model);
            if (saveResult.Item1 && saveResult.Item2) {
                
            }
        } else if (Mode == ModeInScene.Edit)
        {
            Debug.Log("Model was changed, but its in edit mode, so nothing will change");

        }

    }

    public void SetData(ACTPartModel model)
    {
        Debug.Log($"Set act part data {Edit != null}");
        if (Edit != null)
        {
            var controller = Edit.gameObject.GetComponent<ACTPart_controller>();
            if (controller != null)
            {
                controller.SetData(model);
            }
        }
    }

    public void SetData(string developmentId, int level)
    {
        Debug.Log($"Set empty act part data {Edit != null} object {ObjectId()}");

        var model = new ACTPartModel
        {
            parentObjId = null,
            objId = ObjectId(),
            developmentId = developmentId,
            level = level
        };
        SetData(model);
    }
}
