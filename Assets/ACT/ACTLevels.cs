using Lean.Gui;
using Newtonsoft.Json;
using RTS_Cam;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ACTLevels: MonoBehaviour
{
    [SerializeField] private Content Content;
    [SerializeField] private GameObject ACTLevelPrefab;

    [SerializeField] private List<ACTLevel> Levels = new();

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
    }

    public void OnReturnBack()
    {
        StartCoroutine(RiseUp());
    }

    IEnumerator RiseUp()
    {
        Debug.Log($"Double clicked, lets dive into {System.DateTime.Now}");

        ACTSession.Instance.Clear();

        Global.Instance.ShowLoadingScene();

        // https://docs.unity3d.com/ScriptReference/AsyncOperation-allowSceneActivation.html
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        asyncOperation.allowSceneActivation = true;

        yield return null;
    }
}
