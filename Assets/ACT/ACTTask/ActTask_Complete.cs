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

    public void OnSubmit()
    {
        var validatedTasks = tasksToComplete.ValidatedTasks();
        if (validatedTasks == null || validatedTasks.Count == 0)
        {
            return;
        }

        Debug.Log($"Send {validatedTasks.Count} tasks to the blockchain");
    }
}
