using Lean.Gui;
using Rundo.RuntimeEditor.Behaviours.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebBusWindow : MonoBehaviour
{
    private static WebBusWindow _instance;

    [SerializeField] private LeanWindow LeanWindow;
    [SerializeField] private SelectedItem SelectedInputItem;
    [SerializeField] private SelectedItem SelectedOutputItem;

    public static WebBusWindow Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<WebBusWindow>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        SelectedOutputItem.ShowDefault();
        SelectedInputItem.ShowDefault();
    }

    public void ShowWindow()
    {
        if (!LeanWindow.On)
        {
            LeanWindow.TurnOn();
        }
    }

    public void HideWindow()
    {
        //SelectedInputItem.ShowDefault();
        //SelectedOutputItem.ShowDefault();
        LeanWindow.TurnOff();
    }


    public void ShowInput(ProjectItemMetaData metaData)
    {
        ShowWindow();
        SelectedInputItem.Show(metaData, btnEnabled: true);
        if (!SelectedOutputItem.IsShowing()) {
            SelectedOutputItem.ShowDefault();
        }
    }

    public void ShowOutput(ProjectItemMetaData metaData)
    {
        ShowWindow();
        SelectedOutputItem.Show(metaData, btnEnabled: true);
        if (!SelectedInputItem.IsShowing())
        {
            SelectedInputItem.ShowDefault();
        }
    }

    public void OnCancelInput()
    {
        AraFrontend.Instance.OnDeselectInputItem(SelectedInputItem.Data);
        SelectedInputItem.ShowDefault();
    }

    public void OnCancelOutput()
    {
        AraFrontend.Instance.OnDeselectOutputItem(SelectedOutputItem.Data);
        SelectedOutputItem.ShowDefault();
    }
}
