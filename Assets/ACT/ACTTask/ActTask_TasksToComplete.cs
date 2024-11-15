using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Whisper;
using Whisper.Utils;
using Debug = UnityEngine.Debug;

public class ActTask_TasksToComplete : MonoBehaviour
{
    /// <summary>
    /// When user adds a task by typing, then its in single mode.
    /// When user adds a task by recording his audio, then its in record mode.
    /// </summary>
    private bool _singleMode = true;

    [Header("Tasks To Complete Window")]
    [SerializeField] private GameObject LoadingLayer;
    [SerializeField] private Content Content;
    [SerializeField] private GameObject TaskFormPrefab;

    [Header("Other Task Windows")]
    [SerializeField] private ActivityState TaskButton;
    [SerializeField] private ActTask_NewTask NewTaskWindow;
    [SerializeField] private ActTask_Complete CompleteWindow;

    private List<ActTask_TaskForm> TaskFormList = new();

    [HideInInspector]
    public Action<string, bool> AddTaskCallback;

    // Start is called before the first frame update
    void Start()
    {
        CompleteWindow.tasksToComplete = this;
        NewTaskWindow.AddTaskCallback += OnNewTask;
        ResetToDefault();
    }

    // Add a new task
    private void OnNewTask(string data, bool single)
    {
        if (single)
        {
            var title = "";
            var description = "";
            var rows = data.Split(".");
            if (rows.Length < 2)
            {
                rows = data.Split(';');
                if (rows.Length < 2)
                {
                    rows = data.Split('!');
                    if (rows.Length < 2)
                    {
                        rows = data.Split('?');
                    }
                }
            }
            if (rows.Length < 2)
            {
                title = $"Task #{TaskFormList.Count + 1}";
                description = data;
            } else
            {
                title = rows.First();
                description = string.Join(".", rows.Skip(1));
            }
            Debug.Log($"'{data}' has {rows.Length} data");

            AddNewTask(title, description);
        } else
        {
            Debug.LogWarning("Sorry not yet implemented to parse all (show loading layer)");
        }
    }

    // Delete the task
    public void DeleteTask(ActTask_TaskForm task)
    {
        if (!TaskFormList.Contains(task))
        {
            Debug.Log("Task to delete not found");
            return;
        }

        Destroy(task.gameObject);
        if (!TaskFormList.Remove(task))
        {
            Debug.Log("Task was not removed");
            return;
        }

        for (var i = 0; i < TaskFormList.Count; i++)
        {
            TaskFormList[i].ResetNumber(i+1);
        }

        if (TaskFormList.Count == 0)
        {
            CompleteWindow.ResetToDefault();
        }
    }

    public void TaskEdited(ActTask_TaskForm task)
    {
        var valid = task.TaskForm().Validate();
        if (string.IsNullOrEmpty(valid))
        {
            CompleteWindow.SetStatus(true);
        } else
        {
            CompleteWindow.SetStatus(false);
        }
    }

    /// <summary>
    /// Validate all tasks
    /// </summary>
    /// <returns></returns>
    public List<TaskForm> ValidatedTasks()
    {
        var validatedTasks = new List<TaskForm>();
        for (var i = 0; i < TaskFormList.Count; i++)
        {
            var taskForm = TaskFormList[i].TaskForm();
            var err = taskForm.Validate();
            if (!string.IsNullOrEmpty(err))
            {
                Notification.Instance.Show($"#{i+1} task: {err}");
                return null;
            }
            validatedTasks.Add(taskForm);
        }

        return validatedTasks;
    }

    private void AddNewTask(string title, string description)
    {
        var taskForm = Content.Add<ActTask_TaskForm>(TaskFormPrefab);
        TaskFormList.Add(taskForm);
        taskForm.Show(this, TaskFormList.Count, title, description);
    }

    public void TasksAdded()
    {
        Notification.Instance.Show("Tasks were added to the blockchain!");
        TaskButton.ToggleSelect(enabled: false);
        ResetToDefault();
    }


    public void ResetToDefault()
    {
        LoadingLayer.SetActive(false);
        Content.Clear();
    }

}
