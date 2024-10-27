using Dreamteck.Splines;
using Rundo.RuntimeEditor.Behaviours;
using Rundo.RuntimeEditor.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ACTPart_edit;

public class ACTPart : EditorBaseBehaviour, IStateReactor, ACTPart_interface
{
    public enum ModeInScene
    {
        Interactive, // Default mode
        View,   // During the object placeholder, make it view
        DrawLine, // For drawing the line change the select
        Edit, // Same as interactive but hides the transform handler
    }
    [SerializeField]
    protected Canvas Canvas;
    [SerializeField]
    public ActivityState ActivityState;
    [SerializeField]
    protected GameObject Menu;
    [SerializeField]
    protected MouseInput MouseInput;
    [SerializeField] 
    protected Transform SplinePositionersContent;
    [SerializeField]
    protected List<Node> Connections = new();
    [SerializeField]
    protected ACTPart_edit Edit;

    public ModeInScene Mode = ModeInScene.View;
    protected DataGameObjectId objectId;

    void Awake()
    {
        Edit = gameObject.GetComponent<ACTPart_edit>();
    }

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
        if (Mode != ModeInScene.Interactive && Mode != ModeInScene.Edit)
        {
            Canvas.gameObject.SetActive(false);
        }
    }

    public string ObjectId()
    {
        return objectId.ToStringRawValue();
    }

    public void Activate(DataGameObjectId objectId)
    {
        this.objectId = objectId;
        Mode = ModeInScene.Interactive;
        ActivityState.SetActivityGroup(ACTProjects.Instance.ActivityGroup);
        Menu.SetActive(false);
        ACTLevelScene.Instance.AddPart(this);
        Canvas.worldCamera = ACTProjects.Instance.Camera;
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
}
