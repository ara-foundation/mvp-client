using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class TaskForm
{
    public string title;
    public string description;
    public string deadline;
    public decimal price_usd;
    public decimal est_hours;

    public string Validate()
    {
        if (string.IsNullOrEmpty(title))
        {
            return "Title is empty";
        }
        if (string.IsNullOrEmpty(description))
        {
            return "Description is empty";
        }
        if (string.IsNullOrEmpty(deadline))
        {
            return "Deadline is empty";
        }
        var nums = deadline.Split("/");
        if (nums.Length != 3)
        {
            return "Deadline must be dd/mm/yyyy";
        }
        if (price_usd <= 0)
        {
            return "Price is empty";
        }
        if (est_hours <= 0)
        {
            return "Estimated hours is empty";
        }

        return null;
    }
}

public class ActTask_TaskForm : MonoBehaviour
{
    private TaskForm _taskForm;
    private ActTask_TasksToComplete tasksToComplete;

    [SerializeField] private TextMeshProUGUI Number;
    [Header("Form")]
    [SerializeField] private TMP_InputField Title;
    [SerializeField] private TMP_InputField Description;
    [SerializeField] private TMP_InputField Deadline;
    [SerializeField] private TMP_InputField PriceUsd;
    [SerializeField] private TMP_InputField EstHours;
    [SerializeField] private Button DeleteButton;

    public void Show(ActTask_TasksToComplete tasksToComplete, int number, string title, string description)
    {
        this.tasksToComplete = tasksToComplete;
        Number.text = number.ToString();
        _taskForm = new TaskForm()
        {
            title = title,
            description = description,
        };
        Title.text = title;
        Description.text = description;
    }

    public void OnDelete()
    {
        this.tasksToComplete.DeleteTask(this);
    }

    public void ResetNumber(int number)
    {
        Number.text = number.ToString();
    }

    public TaskForm TaskForm()
    {
        return _taskForm;
    }

    #region formchange

    public void OnTitleChange(string data)
    {
        _taskForm.title = data;
    }
    
    public void OnDescriptionChange(string data)
    {
        _taskForm.description = data;
    }

    public void OnDeadlineChange(string deadline)
    {
        _taskForm.deadline = deadline;
    }

    public void OnPriceUsdChange(string price_usd)
    {
        _taskForm.price_usd = decimal.Parse(price_usd);
    }

    public void OnEstHoursChange(string est_hours)
    {
        _taskForm.est_hours = decimal.Parse(est_hours);
    }

    #endregion formchange
}
