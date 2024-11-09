using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    private static Global _instance;
    [SerializeField] private LoadingLayer LoadingLayer;

    public static Global Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Global>();
            }
            return _instance;
        }
    }

    public void ShowLoadingScene()
    {
        LoadingLayer.Show();
    }

    public void ShowStartingScene() {
        var title = $"Starting a scene.{Environment.NewLine}Please wait...";
        ShowStartingScene(title);
    }
    public void ShowStartingScene(string title)
    {
        LoadingLayer.Show(title, enableSpinner: true);
    }

    public void ProgressLoadingTitle(string title)
    {
        LoadingLayer.SetTitle(title);
    }

    public void HideLoadingScene()
    {
        LoadingLayer.Hide();
    }
}
