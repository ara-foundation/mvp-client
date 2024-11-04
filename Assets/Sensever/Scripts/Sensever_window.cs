using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sensever window includes Sensever mesh and a dialogue box
/// </summary>
public class Sensever_window: MonoBehaviour
{
    [SerializeField] private GameObject SenseverMesh;
    [SerializeField] private Sensever_dialogue SenseverDialogue;
    [SerializeField] private LeanWindow LeanWindow;

    private Action<int> textEndCallback;
    private Action<int> textStartCallback;
    private Func<int, bool> hideInsteadContinue;
    private int nextStep = Sensever_dialogue.None;

    private static Sensever_window _instance;

    public static Sensever_window Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Sensever_window>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        SenseverMesh.SetActive(false);
    }

    public void ShowSensever(List<TutorialText> texts, Action<int> textEndCallback, Action<int> textStartCallback, Func<int, bool> hideInsteadContinue)
    {
        nextStep = Sensever_dialogue.None;
        SenseverDialogue.TutorialTexts = texts;
        this.textEndCallback = textEndCallback;
        this.textStartCallback = textStartCallback;
        this.hideInsteadContinue = hideInsteadContinue;
        LeanWindow.Set(true);
    }

    public void ContinueSensever(int nextStep)
    {
        this.nextStep = nextStep;
        if (!LeanWindow.On)
        {
            LeanWindow.Set(true);
        } else
        {
            OnToggle();
        }
    }

    public void HideSensever()
    {
        LeanWindow.Set(false);
    }

    /// <summary>
    /// Along with the OffTogle its a callback to activate sensever after its window is open
    /// </summary>
    public void OnToggle()
    {
        if (!SenseverMesh.activeSelf)
        {
            SenseverMesh.SetActive(true);
        }
        if (this.nextStep != Sensever_dialogue.None)
        {
            SenseverDialogue.ShowAt(this.nextStep);
            this.nextStep = Sensever_dialogue.None;
        } else
        {
            SenseverDialogue.StartTutorial(this.textEndCallback, this.textStartCallback, this.hideInsteadContinue);
        }
    }

    /// <summary>
    /// Along with the OnToggle its a callback to deactivate sensever after its window is closed
    /// </summary>
    public void OffToggle()
    {

        SenseverDialogue.CancelTexting();
        SenseverMesh.SetActive(false);
    }
}
