using Dreamteck.Splines;
using Newtonsoft.Json;
using Rundo.RuntimeEditor.Behaviours;
using Rundo.RuntimeEditor.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SplineComputer))]
public class ACTPart_line : ACTPart, ACTPart_interface
{
    private SplineComputer spline;
    [SerializeField]
    public List<string> partList;

    private void Awake()
    {
        spline = GetComponent<SplineComputer>();
    }

    public new void Activate(DataGameObjectId objectId)
    {
        this.objectId = objectId;
        Load();
        ResetPoints();
        
        Mode = ModeInScene.View;
        ActivityState.SetActivityGroup(ACTLevelScene.Instance.ActivityGroup);
        ACTLevelScene.Instance.AddPart(this);

        if (partList.Count > 0)
        {
            for(var i = 0; i < partList.Count; i++)
            {
                var part = ACTLevelScene.Instance.GetPart(partList[i]);
                if (part == null)
                {
                    Debug.LogError($"the part {partList[i]} is not found in the scene. The line {objectId} can not be drawn");
                    return;
                }
                var actPart = part as ACTPart;
                var defaultMode = actPart.Mode;
                actPart.Mode = ModeInScene.DrawLine;
                SetPoint(part as ACTPart);
                actPart.Mode = defaultMode;
            }

            // Draw to render the line.
            spline.RebuildImmediate();
        }
    }

    private void OnDestroy()
    {
        if (Mode == ModeInScene.Interactive)
        {
            Menu.SetActive(false);
        }
        ACTLevelScene.Instance.RemovePart(this);
        DeletePoints();
    }

    public new void Select(bool enabled)
    {
        return;
    }

    private void Save()
    {
        ACTLevelScene.Instance.OnLineEditingEnd(objectId, partList);
    }

    private void Load()
    {
        if (objectId == null || string.IsNullOrEmpty(objectId.ToStringRawValue()))
        {
            partList = new List<string>();
            return;
        }
        partList = ACTLevelScene.Instance.LineConnections(objectId);
        if (partList == null)
        {
            partList = new List<string>();
        }
    }

    public new void SetData(ACTPartModel _)
    {

    }

    public new void SetData(string _devId, int _lvl)
    {

    }


    ////////////////////////////////////////////////////
    ///
    ///  Change the mode of part in the scene
    ///
    ////////////////////////////////////////////////////
    public new void Interactive(bool on)
    {
        return;
    }

    public new void SetLineMode(bool on)
    {
        return;
    }


    //////////////////////////////////////////////////
    ///
    /// Line methods
    ///
    //////////////////////////////////////////////////

    public void ResetPoints()
    {
        spline.SetPoints(new SplinePoint[] {});
    }

    public void DeletePoints()
    {
        if (partList == null)
        {
            return;
        }
        Load();
        foreach (var partId in partList)
        {
            var objectId = DataGameObjectId.Create(partId);
            var part = RuntimeEditorController.DataSceneBehaviour.Find(objectId);
            var actPart = part.gameObject.GetComponent<ACTPart>();
            actPart.DeleteLastSplinePositioner();
        }
    }

    private int SetPoint(ACTPart part)
    {
        var pointIndex = spline.GetPoints().Length;

        // First, let's create point. The point location must be at the part positon.
        // For this, let's get it from part itself.
        //
        // Then, add a point for a spline at the positon of the part.
        //
        // Finally connect the part to the point.
        var partPosition = part.LinePointPosition();
        var point = new SplinePoint(partPosition);
        spline.SetPoint(pointIndex, point);
        part.ConnectToLine(spline, pointIndex);

        return pointIndex;
    }

    /// <summary>
    /// Set the project part in the scene as the point of the line.
    /// </summary>
    /// <param name="part"></param>
    /// <returns>True if the line is closed</returns>
    public bool OnSetPoint(ACTPart part)
    {
        if (partList.Contains(part.ObjectId()))
        {
            return false;
        }
        // For internal tracking by this spline
        partList.Add(part.ObjectId());

        var pointIndex = SetPoint(part);

        // Draw to render the line.
        spline.RebuildImmediate();

        if (pointIndex == 0) {
            return false;
        } else
        {
            Save();
            return true;
        }
    }
}
