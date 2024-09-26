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

    private string currentUrl = default;
    private Dictionary<string, AraIdeas> cache = new ();

    // Start is called before the first frame update
    async void Start()
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
        } else
        {
            Debug.LogWarning("Failed to fetch ideas from Ara Server");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
