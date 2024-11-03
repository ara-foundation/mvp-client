using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

public class Logos : MonoBehaviour
{
    [SerializeField] private GameObject ideaPlaceholder;
    [SerializeField] private GameObject ideaCard;
    [SerializeField] private GameObject content;

    private static Logos _instance;

    public static Logos Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Logos>();
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    async void OnEnable()
    {
        await LoadIdeas();
    }

    public async Task LoadIdeas()
    {
        ClearContent();
        var result = await FetchIdeas();
        if (result != null && result.data != null && result.data.Count > 0)
        {
            Instantiate(ideaPlaceholder, content.transform);
            foreach (var data in result.data)
            {
                var res = Instantiate(ideaCard, content.transform);
                CardLogos cardLogos = res.GetComponent<CardLogos>();
                cardLogos.Show(data);
            }
        }
        else
        {
            Notification.Instance.Show("Failed to fetch logos ideas from the ARA Server");
            Debug.LogWarning("Failed to fetch ideas from Ara Server");
        }
    }

    private void ClearContent()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        };
    }

    private async Task<AraIdeas> FetchIdeas()
    {
        AraIdeas incorrectResult = new();

        string url = NetworkParams.AraActUrl + "/logos/ideas";

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

        AraIdeas result;
        try
        {
            result = JsonConvert.DeserializeObject<AraIdeas>(res);
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
