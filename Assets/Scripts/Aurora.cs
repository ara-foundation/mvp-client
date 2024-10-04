using Lean.Gui;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Aurora : MonoBehaviour
{
    private static Aurora _instance;
    private Aurora_NewUserScenario AuroraNewUserScenario;
    [SerializeField] private GameObject[] AuroraBackgrounds;
    [SerializeField] private GameObject Content;
    [SerializeField] private GameObject UserScenarioCard;
    [SerializeField] private LeanToggle TopBarAurora;
    private bool newScenarioMode = false;

    public static Aurora Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Aurora>();
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        AuroraNewUserScenario = gameObject.GetComponent<Aurora_NewUserScenario>();
        if (AuroraNewUserScenario == null)
        {
            throw new Exception("Aurora_NewUserScenario not found");
        }
    }

    public void ResetNewScenarioMode()
    {
        newScenarioMode = false;
    }

    private void ClearContent()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        };
    }

    private void OnDisable()
    {
        ResetNewScenarioMode();
        HideUserScenarios();
    }

    public async void Show()
    {
        await ShowUserScenarios();
    }

    public async Task ShowUserScenarios()
    {
        if (newScenarioMode)
        {
            return;
        }
        ResetNewScenarioMode();
        ClearContent();
        // Show the User Scenarios background (a blue color)
        // Otherwise it will show the background in tavern for the new scenario
        if (AuroraBackgrounds != null && AuroraBackgrounds.Length > 0)
        {
            for (int i = 0; i < AuroraBackgrounds.Length; i++)
            {
                AuroraBackgrounds[i].gameObject.SetActive(true);
            }
        }
        var result = await FetchUserScenarios();
        if (result != null && result.Count > 0)
        {
            foreach (var data in result)
            {
                var res = Instantiate(UserScenarioCard, Content.transform);
                var cardAurora = res.GetComponent<CardAurora>();
                cardAurora.Show(data);
            }
        }
        else
        {
            Debug.LogWarning("Failed to fetch user scenarios from Ara Server");
        }
    }

    private async Task<List<UserScenarioInServer>> FetchUserScenarios()
    {
        List<UserScenarioInServer> incorrectResult = new();

        string url = NetworkParams.AraActUrl + "/aurora/user-scenarios";

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

        List<UserScenarioInServer> result;
        try
        {
            result = JsonConvert.DeserializeObject<List<UserScenarioInServer>>(res);
        }
        catch (Exception e)
        {
            Debug.LogError(e + " for " + res);
            return incorrectResult;
        }
        return result;
    }

    public void HideUserScenarios()
    {
        if (AuroraBackgrounds != null && AuroraBackgrounds.Length > 0)
        {
            for (int i = 0; i < AuroraBackgrounds.Length; i++)
            {
                if (AuroraBackgrounds[i].gameObject)
                {
                    AuroraBackgrounds[i].gameObject.SetActive(false);
                }
            }
        }
        ClearContent();
    }

    public void TestLogos()
    {
        HideUserScenarios();
        Debug.LogWarning("The Logos idea must come from the server... for testing we use local version");
        var logos = new AraDiscussion()
        {
            type = "comment",
            id = 1,
            attributes = new AraDiscussionAttributes() {
                title = "Sample idea",
                slug = "",
                commentCount = 0,
                participantCount = 0,
                createdAt = "",
                lastPostedAt = "",
                lastPostNumber = 0,
            },
            relationships = new AraDiscussionRelationships(){
                user = new IncludedUser(){ 
                    attributes = new UserAttributes()
                    {
                        displayName = "User Name",
                        username = "username"
                    }
            },
            firstPost = new IncludedPost (){ 
                attributes = new PostAttributes()
                {
                    contentHtml = "Here is the sample idea parameters",
                    createdAt = "2023:12"
                }
            }
            }
};
        NewScenario(logos);
    }

    public void NewScenario(AraDiscussion logos)
    {
        newScenarioMode = true;
        HideUserScenarios();
        TopBarAurora.TurnOn();
        if (AuroraNewUserScenario == null)
        {
            Notification.Instance.Show("Internal Error: Stupid Medet forgive him for his mistake, he is a human after all");
        }

        AuroraNewUserScenario.Show(logos);
    }

}
