using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    private static Global _instance;
    [SerializeField] private LeanWindow LoadingSceneModal;

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
        LoadingSceneModal.Set(true);
    }
}
