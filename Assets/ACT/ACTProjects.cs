using Lean.Gui;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class ACTPartParams
{
    // server params
    public string projectId;
    public string actId;

    // scene parameters
    // todo: include the scene from the server
    public string prefabUuid;   // to load in the scene

    public string projectName;
    public int level; // how many nested parts are there
    public string techStack;
    public string creatorId;
    public double budget;
    public string maintainer;
}

[Serializable]
public class ActWithProjectAndPlan : ForumParams
{
    public string _id;
    public string project_id;

    public string tech_stack;//: string;
    public string source_code_url;//: string;
    public string test_url;//?: string;
    public int start_time;//: number;
    public string duration;//: number;
#nullable enable
    public int? parts_amount;
#nullable disable
    [SerializeField] public Project[] project_v1;
    [SerializeField] public Plan[] plan;
}


public class ACTProjects : MonoBehaviour
{
    private static ACTProjects _instance;

    public ActivityGroup ActivityGroup;

    public static string TargetTag = "target"; // objects must have this element

    public Camera MainCamera;
    public Camera ACTProjectsCamera;
    [SerializeField] private Canvas UICanvas;
    [SerializeField] private GameObject[] Objects;

    [SerializeField] private GameObject ACTProjectPrefab;
    [SerializeField] private Content Content;
    

    public static ACTProjects Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ACTProjects>();
            }
            return _instance;
        }
    }

    private async void OnEnable()
    {
        if (MainCamera && MainCamera.gameObject.activeSelf)
        {
            MainCamera.gameObject.SetActive(false);
        }
        if (ACTProjectsCamera && !ACTProjectsCamera.gameObject.activeSelf)
        {
            ACTProjectsCamera.gameObject.SetActive(true);
            UICanvas.worldCamera = ACTProjectsCamera;
            AraFrontend.Instance.MainCamera = ACTProjectsCamera;
        }
        
        for (var i = 0; i < Objects.Length; i++)
        {
            if (Objects[i])
            {
                Objects[i].SetActive(true);
            }
        }


        await LoadACTProjects();
    }

    private void OnDisable()
    {
        if (ACTProjectsCamera && ACTProjectsCamera.gameObject.activeSelf)
        {
            ACTProjectsCamera.gameObject.SetActive(false);
        }
        if (MainCamera && !MainCamera.gameObject.activeSelf)
        {
            UICanvas.worldCamera = MainCamera;
            MainCamera.gameObject.SetActive(true);
            AraFrontend.Instance.MainCamera = MainCamera;
        }

        for (var i = 0; i < Objects.Length; i++)
        {
            if (Objects[i])
            {
                Objects[i].SetActive(false);
            }
        }
    }

    public async Task LoadACTProjects()
    {
        Content.Clear();
        var result = await FetchACTProjects();
        if (result != null && result.Length > 0)
        {
            foreach (var data in result)
            {
                var res = Content.Add(ACTProjectPrefab);
                ACTProject project = res.GetComponent<ACTProject>();
                project.Show(data);
            }
        }
        else
        {
            Debug.LogWarning("Failed to fetch ideas from Ara Server");
        }
    }

    private async Task<ActWithProjectAndPlan[]> FetchACTProjects()
    {
        ActWithProjectAndPlan[] incorrectResult = new ActWithProjectAndPlan[] { };

        string url = NetworkParams.AraActUrl + "/act/projects";

        string res;
        try
        {
            res = await WebClient.Get(url);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return incorrectResult;
        }

        ActWithProjectAndPlan[] result;
        try
        {
            result = JsonConvert.DeserializeObject<ActWithProjectAndPlan[]>(res);
        }
        catch (Exception e)
        {
            Debug.LogError(e + " for " + res);
            return incorrectResult;
        }
        return result;
    }

    public async Task<AraDiscussion> FetchIdea(int id)
    {
        AraDiscussion incorrectResult = new();

        string url = NetworkParams.AraActUrl + $"/logos/idea/{id}";

        string res;
        try
        {
            res = await WebClient.Get(url);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return incorrectResult;
        }

        AraDiscussion result;
        try
        {
            result = JsonConvert.DeserializeObject<AraDiscussion>(res);
        }
        catch (Exception e)
        {
            Debug.LogError(e + " for " + res);
            return incorrectResult;
        }
        return result;
    }
}
