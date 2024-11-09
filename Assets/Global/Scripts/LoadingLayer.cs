using Assets.SimpleSpinner;
using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(LeanWindow))]
public class LoadingLayer : MonoBehaviour
{
    private LeanWindow leanWindow;
    [SerializeField] private TextMeshProUGUI TitleLabel;
    [SerializeField] private SimpleSpinner Spinner;
    public readonly string DefaultTitle = "Loading Scene...";
    private void Awake()
    {
        leanWindow = GetComponent<LeanWindow>();
        TitleLabel.text = DefaultTitle;
    }

    public void Show()
    {
        Spinner.enabled = false;
        TitleLabel.text = DefaultTitle;
        leanWindow.TurnOn();
    }

    public void Show(string title, bool enableSpinner)
    {
        TitleLabel.text = title;
        leanWindow.TurnOn();
        Spinner.enabled = enableSpinner;
    }

    public void SetTitle(string title)
    {
        if (leanWindow.On)
        {
            TitleLabel.text = title;
        }
    }

    public void Hide()
    {
        Spinner.enabled = false;
        leanWindow.TurnOff();
    }
}
