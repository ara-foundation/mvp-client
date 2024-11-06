using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityGroup : MonoBehaviour
{
    private List<ActivityState> activityStates = new();
    [SerializeField] private CameraFocus CameraFocus;
    [SerializeField] private TransformSelection TransformSelection;

    public void SetActivityState(ActivityState activityState)
    {
        activityStates.Add(activityState);
    }

    public void DeleteActivityState(ActivityState activityState)
    {
        activityStates.Remove(activityState);
    }

    // Start is called before the first frame update
    public void Select(ActivityState from, bool enabled)
    {
        if (!activityStates.Contains(from))
        {
            return;
        }

        if (CameraFocus != null)
        {
            CameraFocus.SelectTarget(from.gameObject.transform, enabled);
        }
        if (TransformSelection != null)
        {
            TransformSelection.SelectTarget(from.gameObject.transform, enabled);
        }

        UnselectAllOthers(from);
    }

    /// <summary>
    /// If any activity state is selected, then it will find it and call unselect
    /// </summary>
    public void UnSelectAll()
    {
        foreach (var activityState in activityStates)
        {
            if (activityState != null && activityState.Mode == StateMode.Selected) {
                activityState.ToggleSelect(enabled: false);
                break;
            }
        }
    }

    private void UnselectAllOthers(ActivityState from)
    {
        foreach (var activtyState in activityStates)
        {
            if (activtyState == from)
            {
                continue;
            }

            activtyState.ChangeMode(StateMode.None);
        }
    }

}
