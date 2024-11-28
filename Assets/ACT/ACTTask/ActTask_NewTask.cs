using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Whisper;
using Whisper.Utils;
using Debug = UnityEngine.Debug;

public class ActTask_NewTask : MonoBehaviour
{
    /// <summary>
    /// When user adds a task by typing, then its in single mode.
    /// When user adds a task by recording his audio, then its in record mode.
    /// </summary>
    private bool _singleMode = true;

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

    private void Awake()
    {
        whisper.OnNewSegment += OnNewSegment;
        whisper.OnProgress += OnProgressHandler;
        whisper.language = "en";
        whisper.translateToEnglish = false;
        whisper.dropOldBuffer = true;

        microphoneRecord.OnVadChanged += OnVadChanged;

        microphoneRecord.OnRecordStop += OnMicrophoneStop;
    }

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
        ResetToDefault();
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
        if (microphoneRecord.IsRecording)
        {
            OnStopRecord();
        }

        AddButton.interactable = true;
        _singleMode = true;
        SetAddButtonLabel();
        ToggleRecordButton(play: true);
        TasksInput.text = "";

        ProgressLabel.text = "";
        ClearRecordButton.gameObject.SetActive(false);
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
            Notification.Show("Task body is empty");
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
