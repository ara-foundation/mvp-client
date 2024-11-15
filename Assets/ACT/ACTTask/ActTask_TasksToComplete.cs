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
    [SerializeField] private ActTask_NewTask NewTaskWindow;

    private List<ActTask_TaskForm> TaskFormList = new();

    [Header("Buttons")]
    [SerializeField] private Button AddButton;
    [SerializeField] private TextMeshProUGUI AddButtonLabel;
    public readonly string AddButtonSingle = "Add A Task";
    public readonly string AddButtonRecord = "Add All Tasks";
    [Space(5)]
    [Header("Recorder")]
    [SerializeField] private Button PlayRecordButton;
    [SerializeField] private Button StopRecordButton;
    [SerializeField] private TextMeshProUGUI ProgressLabel;
    public bool streamSegments = true;
    public WhisperManager whisper;
    public MicrophoneRecord microphoneRecord;
    private string _buffer;

    [Header("Input Field")]
    [SerializeField] private TMP_InputField TasksInput;
    [SerializeField] private Button ClearRecordButton;

    [HideInInspector]
    public Action<string, bool> AddTaskCallback;


    private void OnVadChanged(bool isSpeechDetected)
    {
        var status = "no audio";
        if (isSpeechDetected)
        {
            status = "audio detected";
        }
        ProgressLabel.text = $"Recording [{status}]";
    }

    private void OnProgressHandler(int progress)
    {
        ProgressLabel.text = $"Parsing the speech: {progress}%";
    }

    // Start is called before the first frame update
    void Start()
    {
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
    }

    private void AddNewTask(string title, string description)
    {
        var taskForm = Content.Add<ActTask_TaskForm>(TaskFormPrefab);
        TaskFormList.Add(taskForm);
        taskForm.Show(this, TaskFormList.Count, title, description);
    }

    private void SetAddButtonLabel()
    {
        if (_singleMode)
        {
            AddButtonLabel.text = AddButtonSingle;
        } else
        {
            AddButtonLabel.text = AddButtonRecord;
        }
    }

    private void ToggleRecordButton(bool play)
    {
        PlayRecordButton.gameObject.SetActive(play);
        PlayRecordButton.interactable = play;
        StopRecordButton.gameObject.SetActive(!play);
    }

    public void ResetToDefault()
    {
        LoadingLayer.SetActive(false);
        Content.Clear();
    }

    private void OnNewSegment(WhisperSegment segment)
    {
        if (!streamSegments)
            return;

        _buffer += segment.Text;
        TasksInput.text = _buffer + "...";
        //UiUtils.ScrollDown(scroll);
    }

    public void OnStopRecord()
    {
        Debug.Log($"Stop the record, is microphone is recording? {microphoneRecord.IsRecording}");
        if (microphoneRecord.IsRecording)
        {
            microphoneRecord.StopRecord();
        }
    }

    public void OnStartRecord()
    {
        AddButton.interactable = false;
        _singleMode = false;
        TasksInput.text = "";
        ClearRecordButton.gameObject.SetActive(false);
        OnVadChanged(isSpeechDetected: false);
        microphoneRecord.StartRecord();
        ToggleRecordButton(play: false);
        SetAddButtonLabel();
    }

    public void OnAdd()
    {
        if (string.IsNullOrEmpty(TasksInput.text))
        {
            Notification.Instance.Show("Task body is empty");
            return;
        }

        AddTaskCallback?.Invoke(TasksInput.text, _singleMode);
        Debug.Log($"tasks {TasksInput.text}, is single mode? {_singleMode}");
        ResetToDefault();
    }

    private async void OnMicrophoneStop(AudioChunk recordedAudio)
    {
        Debug.Log($"OnMicrophone stop");
        _buffer = "";

        ToggleRecordButton(play: true);
        PlayRecordButton.interactable = false;

        var sw = new Stopwatch();
        sw.Start();

        var res = await whisper.GetTextAsync(recordedAudio.Data, recordedAudio.Frequency, recordedAudio.Channels);
        if (res == null)
            return;

        ClearRecordButton.gameObject.SetActive(true);
        AddButton.interactable = true;
        PlayRecordButton.interactable = true;

        var time = sw.ElapsedMilliseconds;
        var rate = recordedAudio.Length / (time * 0.001f);
        ProgressLabel.text = $"Time: {time} ms\nRate: {rate:F1}x";

        var text = res.Result;
        //if (printLanguage)
        //    text += $"\n\nLanguage: {res.Language}";

        TasksInput.text = text;
        //UiUtils.ScrollDown(scroll);
    }
}
