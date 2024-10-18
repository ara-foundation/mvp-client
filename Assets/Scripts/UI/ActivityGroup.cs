using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityGroup : MonoBehaviour
{
    [SerializeField]
    private List<ActivityState> activityStates = new();

    public void SetActivityState(ActivityState activityState)
    {
        Debug.Log($"ACT Project is setting the activity state {activityState.GetInstanceID()}");
        activityStates.Add(activityState);
    }

    public void DeleteActivityState(ActivityState activityState)
    {
        activityStates.Remove(activityState);
    }

    // Start is called before the first frame update
    public void ChangeActivity(ActivityState from, StateMode mode)
    {
        if (!activityStates.Contains(from))
        {
            return;
        }

        if (mode != StateMode.Selected)
        {
            return;
        }

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
