using Lean.Gui;
using Newtonsoft.Json;
using RTS_Cam;
using Rundo.RuntimeEditor.Behaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ACTLevels: EditorBaseBehaviour
{
    [SerializeField] private Content Content;
    [SerializeField] private GameObject ACTLevelPrefab;

    [SerializeField] private List<ACTLevel> Levels = new();

    private ACTLevel currentLevel = null;

    private static ACTLevels _instance;

    public static ACTLevels Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ACTLevels>();
            }
            return _instance;
        }
    }

    private void OnEnable()
    {
        Content.Clear();

        if (ACTSession.Instance == null || ACTSession.Instance.Level <= 0)
        {
            return;
        }
        // for now we have only one element
        var obj = Instantiate(ACTLevelPrefab, Content.transform);
        var level = obj.GetComponent<ACTLevel>();
        level.Set(ACTSession.Instance.Level, true, ACTSession.Instance.Project.project_v1[0].project_name);
        Levels.Add(level);
        currentLevel = level;
    }

    public void OnReturnBack()
    {
        if (Levels.Count > 0)
        {
            StartCoroutine(RiseUp());
        }
    }

    public void OnSceneUpdate(Action OnSave)
    {
        if (currentLevel == null)
        {
            return;
        }

        currentLevel.SetSave(OnSave);
    }

    IEnumerator RiseUp()
    {
        ACTSession.Instance.Clear();
        Global.Instance.ShowLoadingScene();

        yield return new WaitForSeconds(0.2f);

        // https://docs.unity3d.com/ScriptReference/AsyncOperation-allowSceneActivation.html
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
