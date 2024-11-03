using Newtonsoft.Json;
using Rundo.RuntimeEditor.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ACTSession : MonoBehaviour
{
    private static ACTSession _instance;

    public ActWithProjectAndPlan Development = null;
    /// <summary>
    /// Indicates the current level
    /// </summary>
    public int Level = 0;
    public string DevelopmentId { get { return Development._id; } }

    private List<string> parentObjectId = new List<string>();
    private List<ACTScene> scenes = new ();
    private List<ACTPartModel[]> partsInScene = new ();
    private List<string> names = new ();

    public static ACTSession Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ACTSession>();
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetFirstLevel(ActWithProjectAndPlan _development)
    {
        Development = _development;
        Level = 1;
        names.Add(_development.project_v1[0].project_name);
        parentObjectId.Add(null);   // First level has no parent
    }

    /// <summary>
    /// When a part is clicked to enter, let's this part
    /// </summary>
    /// <param name="partId"></param>
    public void AddLevel(DataGameObjectId partId, string name)
    {
        Level++;
        parentObjectId.Add(partId.ToStringRawValue());
        names.Add(name);
    }

    public void Clear()
    {
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Returns the current scene parameters
    /// </summary>
    /// <returns></returns>
    public string Name(int level)
    {
        if (names.Count >= level)
        {
            return names[level - 1];
        }
        return "";
    }

    /// <summary>
    /// Returns the current scene parameters
    /// </summary>
    /// <returns></returns>
    public async Task<ACTScene> Scene(int level)
    {
        if (scenes.Count >= level)
        {
            return scenes[level - 1];
        }
        var scene = await FetchScene();
        scenes.Add(scene);

        return scene;
    }

    public string CurrentParentObjectId()
    {
        return parentObjectId[Level - 1];
    }

    public ACTPartModel CurrentPart()
    {
        if (Level < 2)
        {
            return null;
        }

        var myObjId = parentObjectId[Level - 1];
        var parentLevel = Level - 1;
        var parentParts = partsInScene[parentLevel - 1];
        for (int i = 0; i < parentParts.Length; i++)
        {
            var part = parentParts[i];
            if (part.objId.Equals(myObjId))
            {
                return part;
            }
        }

        return null;
    }

    public void CurrentPart(ACTPartModel part)
    {
        if (Level < 2)
        {
            return;
        }

        var myObjId = parentObjectId[Level - 1];
        var parentLevel = Level - 1;
        var parentParts = partsInScene[parentLevel - 1];
        for (int i = 0; i < parentParts.Length; i++)
        {
            if (parentParts[i].objId.Equals(myObjId))
            {
                parentParts[i] = part;
                return;
            }
        }
    }


    /// <summary>
    /// Returns the part parameters
    /// </summary>
    /// <returns></returns>
    public async Task<ACTPartModel[]> Parts(int level)
    {
        if (partsInScene.Count >= level)
        {
            return partsInScene[level - 1];
        }
        var parts = await FetchParts();

        partsInScene.Add(parts);

        return parts;
    }

    /// <summary>
    /// Returns the current scene parameters
    /// </summary>
    /// <returns></returns>
    public async Task<ACTScene> CurrentScene()
    {
        return await Scene(Level);
    }

    /// <summary>
    /// Returns the part parameters
    /// </summary>
    /// <returns></returns>
    public async Task<ACTPartModel[]> CurrentParts()
    {
        return await Parts(Level);
    }

    public string CurrentName()
    {
        return Name(Level);
    }

    /// <summary>
    /// Fetch the part parameters for the development id
    /// </summary>
    /// <param name="developmentId"></param>
    /// <returns></returns>
    private async Task<ACTPartModel[]> FetchParts()
    {
        ACTPartModel[] incorrectResult = new ACTPartModel[] { };
        if (Level == 0)
        {
            return incorrectResult;
        }

        string url = NetworkParams.AraActUrl + "/act/parts/" + DevelopmentId;
        if (Level > 1)
        {
            url += $"/{Level}/{parentObjectId[Level - 1]}";
            Debug.Log($"Fetching the nested parts url={url}");
        }

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

        if (string.IsNullOrEmpty(res))
        {
            return incorrectResult;
        }

        ACTPartModel[] result;
        try
        {
            result = JsonConvert.DeserializeObject<ACTPartModel[]>(res);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return incorrectResult;
        }
        return result;
    }

    private async Task<ACTScene> FetchScene()
    {
        ACTScene incorrectResult = new()
        {
            sceneId = "",
        };
        if (Level == 0)
        {
            return incorrectResult;
        }

        string url = NetworkParams.AraActUrl + "/act/scenes/" + DevelopmentId;
        if (Level > 1)
        {
            url += $"/{Level}/{parentObjectId[Level-1]}";
            Debug.Log($"Fetching the nested scene url={url}");
        }

        string res;
        try
        {
            res = await WebClient.Get(url);
        }
        catch (Exception)
        {
            return incorrectResult;
        }

        if (string.IsNullOrEmpty(res))
        {
            return incorrectResult;
        }

        ACTScene result;
        try
        {
            result = JsonConvert.DeserializeObject<ACTScene>(res);
        }
        catch (Exception)
        {
            return incorrectResult;
        }
        return result;
    }
}
