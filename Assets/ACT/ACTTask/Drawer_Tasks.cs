using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Drawer_Tasks : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private LeanWindow Window;
    [Space(20)]
    static public string DefaultTitle = "Tasks (0)";
    [SerializeField] private TextMeshProUGUI Title;
    [SerializeField] private GameObject TaskFormPrefab;
    [SerializeField] private Content Content;

    //private List<TaskForm> tasks = new();
    private static Drawer_Tasks _instance;

    private int callerId;
    public delegate void HideCallback(int callerId);
    public event HideCallback OnHideCallback;

    public static Drawer_Tasks Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Drawer_Tasks>();
            }
            return _instance;
        }
    }

    public void Show(List<TaskForm> tasks, int callerId)
    {
        this.callerId = callerId;
        //this.tasks = tasks;
        SetTitle(tasks.Count);
        Content.Clear();

        for(var i = 0; i < tasks.Count; i++)
        {
            var form = Content.Add<ActTask_TaskForm>(TaskFormPrefab);
            form.Show(i+1, tasks[i]);
        }
        Window.TurnOn();
    }

    public void Hide()
    {
        Window.TurnOff();
        SetTitle();
        Content.Clear();
        OnHideCallback?.Invoke(callerId);
    }

    void Start()
    {
        Hide();
    }
    public void SetTitle(int taskAmount)
    {
        Title.text = $"Tasks ({taskAmount})";
    }

    public void SetTitle()
    {
        Title.text = DefaultTitle;
    }
}
