using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityGroup : MonoBehaviour
{
    private List<ActivityState> activityStates = new();
    [SerializeField] private CameraFocus CameraFocus;

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

        CameraFocus.SelectTarget(from.gameObject.transform, enabled);

        UnselectAllOthers(from);
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
