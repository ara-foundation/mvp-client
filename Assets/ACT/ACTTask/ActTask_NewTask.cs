using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActTask_NewTask : MonoBehaviour
{
    /// <summary>
    /// When user adds a task by typing, then its in single mode.
    /// When user adds a task by recording his audio, then its in record mode.
    /// </summary>
    private bool _singleMode = true;

    [Header("Buttons")]
    [SerializeField] private TextMeshProUGUI AddButtonLabel;
    public readonly string AddButtonSingle = "Add A Task";
    public readonly string AddButtonRecord = "Add All Tasks";
    [Space(5)]
    [Header("Recorder")]
    [SerializeField] private Button PlayRecordButton;
    [SerializeField] private Button StopRecordButton;
    [Header("Input Field")]
    [SerializeField] private TMP_InputField TasksInput;

    [HideInInspector]
    public Action<string, bool> AddTaskCallback;

    // Start is called before the first frame update
    void Start()
    {
        _singleMode = false;
        SetAddButtonLabel();
        ToggleRecordButton(play: true);
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

    public void OnAdd()
    {
        if (_singleMode)
        {
            if (!string.IsNullOrEmpty(TasksInput.text))
            {
                AddTaskCallback?.Invoke(TasksInput.text, _singleMode);
                TasksInput.text = "";
            }
        }
    }
}
