using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SplineComputer))]
public class ACTPart_line : ACTPart
{
    private SplineComputer spline;
    private List<ACTPart> partList = new ();

    private void Awake()
    {
        spline = GetComponent<SplineComputer>();
    }

    public new void Activate()
    {
        Mode = ModeInScene.View;
        ActivityState.SetActivityGroup(ACTProjects.Instance.ActivityGroup);
        Menu.SetActive(false);
        ACTLevelScene.Instance.AddPart(this);
        Canvas.worldCamera = ACTProjects.Instance.Camera;
        Canvas.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        if (Mode == ModeInScene.Interactive)
        {
            ACTLevelScene.Instance.RemovePart(this);
            Menu.SetActive(false);
        }
    }

    public new void Select(bool enabled)
    {
        return;
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
        foreach (var part in partList)
        {
            part.DeleteLastSplinePositioner();
        }
    }

    /// <summary>
    /// Set the project part in the scene as the point of the line.
    /// </summary>
    /// <param name="part"></param>
    /// <returns>True if the line is closed</returns>
    public bool OnSetPoint(ACTPart part)
    {
        if (partList.Contains(part))
        {
            return false;
        }
        var pointIndex = spline.GetPoints().Length;

        // For internal tracking by this spline
        partList.Add(part);

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

        // Draw to render the line.
        spline.RebuildImmediate();


        if (pointIndex == 0) {
            return false;
        } else
        {
            return true;
        }
    }
}
