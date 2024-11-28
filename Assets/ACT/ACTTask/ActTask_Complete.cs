using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActTask_Complete : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Status;
    [SerializeField] private Button SubmitButton;
    [HideInInspector]
    public ActTask_TasksToComplete tasksToComplete;

    readonly string DefaultStatus = "Status: no tasks were created";
    readonly string ListedStatus = "Status: systemize the tasks";
    readonly string ReadyStatus = "Status: ready to submit";

    // Start is called before the first frame update
    void Start()
    {
        ResetToDefault();
    }

    public void ResetToDefault()
    {
        Status.text = DefaultStatus;
        SubmitButton.interactable = false;
    }

    public void SetStatus(bool ready)
    {
        if (!SubmitButton.interactable)
        {
            SubmitButton.interactable = true;
        }
        if (ready)
        {
            Status.text = ReadyStatus;
        } else
        {
            Status.text = ListedStatus;
        }
    }

    public async void OnSubmit()
    {
        var validatedTasks = tasksToComplete.ValidatedTasks();
        if (validatedTasks == null || validatedTasks.Count == 0)
        {
            return;
        }

        if (ACTSession.Instance != null)
        {
            var devId = ACTSession.Instance.DevelopmentId;
            // Tasks are always set within the scene, but for its parent.
            // Therefore, the level is upgraded.
            var lvl = ACTSession.Instance.Level - 1;
            var partId = ACTSession.Instance.CurrentParentObjectId();

            if (lvl < 0)
            {
                Notification.Show("lvl can not be a negative number to submit the task");
                return;
            }

            foreach (var task in validatedTasks)
            {
                task.developmentId = devId;
                task.level = lvl;
                task.parentObjId = partId;
            }

            Global.Instance.ShowStartingScene("Saving the data...");

            bool saved = await ACTSession.Instance.SaveTasks(validatedTasks.ToArray(), devId, lvl, partId);

            Global.Instance.HideLoadingScene();

            if (saved)
            {
                tasksToComplete.TasksAdded();
            }
        } else
        {
            Notification.Show("No ACTSession in the scene");
        }
    }
}
