using Lean.Gui;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static NBitcoin.Scripting.PubKeyProvider;

[Serializable]
public class ForumParams
{
    [SerializeField] public string forum_username;
    [SerializeField] public int forum_user_id;
    [SerializeField] public int forum_discussion_id;
    [SerializeField] public string forum_created_at;
}

[Serializable]
public class PlanRequest: ForumParams
{
    //
    // Generic data
    // 
    [SerializeField] public string leader_username; // the main maintainer
    [SerializeField] public int leader_user_id; // the main maintainer
    [SerializeField] public string project_name; // how the project is named
    [SerializeField] public int logos_id; // the id of the logos
    [SerializeField] public string user_scenario_id; // the matching user scenario

    // 
    // Tech stack
    // 
    [SerializeField] public string tech_stack;
    [SerializeField] public int cost_usd;
    [SerializeField] public int duration; // in days
    [SerializeField] public string source_code_url;
    [SerializeField] public string test_url;

    // 
    // Sangha parameter
    // 
    [SerializeField] public string token_name;
    [SerializeField] public string token_symbol;
    [SerializeField] public string token_max_supply;
    [SerializeField] public string sangha_welcome; // the two sentences to interact other people

    public string Validate()
    {
        if (string.IsNullOrEmpty(project_name))
        {
            return "No project name";
        }
        if (string.IsNullOrEmpty(tech_stack))
        {
            return "No tech stack";
        }
        if (cost_usd <= 0)
        {
            return "No cost specified";
        }
        if (duration <= 0)
        {
            return "No development duration";
        }
        if (string.IsNullOrEmpty(source_code_url))
        {
            return "No source code specified";
        }
        if (string.IsNullOrEmpty(token_name))
        {
            return "No TokenName specified";
        }
        if (string.IsNullOrEmpty(token_symbol))
        {
            return "No TokenSymbol specified";
        }
        if (string.IsNullOrEmpty(token_max_supply))
        {
            return "No TokenMaxSupply specified";
        }
        if (string.IsNullOrEmpty(sangha_welcome))
        {
            return "No SanghaWelcome specified";
        }
        if (string.IsNullOrEmpty(leader_username) || leader_user_id <= 0)
        {
            return "No leader specified";
        }
        if (logos_id <= 0)
        {
            return "No logos that it plans to realize";
        }
        if (string.IsNullOrEmpty(user_scenario_id))
        {
            return "No user scenario that it follows";
        }
        return "";
    }
}

[Serializable]
public class Plan : ForumParams
{
    public string _id;
    public string project_id;
    public string cost_usd;
#nullable enable
    public string? used_budget;
    public string? sangha_welcome;
#nullable disable
}

[Serializable]
public class PlanWithProject: Plan
{
    [SerializeField] public Project[] project_v1;
}


public class Maydone : MonoBehaviour
{
    private static Maydone _instance;
    private Maydone_NewPlan MaydoneNewPlan;
    [SerializeField] private GameObject Content;
    [SerializeField] private GameObject NewPlanContent;
    [SerializeField] private GameObject MaydoneCard;
    [SerializeField] private LeanToggle TopBarMaydone;
    private bool newPlanMode = false;

    public static Maydone Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Maydone>();
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        MaydoneNewPlan = gameObject.GetComponent<Maydone_NewPlan>();
        if (MaydoneNewPlan == null)
        {
            throw new Exception("Maydone_NewPlan not found");
        }
    }

    public void ResetNewPlanMode()
    {
        newPlanMode = false;
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
        ResetNewPlanMode();
        HidePlans();
    }

    public async void Show()
    {
        await ShowPlans();
    }

    public async Task ShowPlans()
    {
        if (newPlanMode)
        {
            return;
        }
        NewPlanContent.SetActive(false);
        ResetNewPlanMode();
        ClearContent();

        var result = await FetchPlansWithProjects();
        if (result != null && result.Count > 0)
        {
            foreach (var data in result)
            {
                var res = Instantiate(MaydoneCard, Content.transform);
                var cardMaydone = res.GetComponent<CardMaydone>();
                cardMaydone.Show(data);
            }
        }
        else
        {
            Debug.LogWarning("Failed to fetch user scenarios from Ara Server");
        }
    }

    private async Task<List<PlanWithProject>> FetchPlansWithProjects()
    {
        List<PlanWithProject> incorrectResult = new();

        string url = NetworkParams.AraActUrl + "/maydone/plans";

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

        List<PlanWithProject> result;
        try
        {
            result = JsonConvert.DeserializeObject<List<PlanWithProject>>(res);
        }
        catch (Exception e)
        {
            Debug.LogError(e + " for " + res);
            return incorrectResult;
        }
        return result;
    }

    public void HidePlans()
    {
        ClearContent();
    }

    private void OnEnable()
    {
        Debug.LogWarning("Todo: Add to Maydone.LeanWindow.On the Maydone.Show()");
        //TestLogos();
    }

    public void TestLogos()
    {
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
        NewPlan(logos, null);
    }

    public void NewPlan(AraDiscussion logos, UserScenarioInServer userScenario)
    {
        newPlanMode = true;
        NewPlanContent.SetActive(true);

        HidePlans();
        TopBarMaydone.TurnOn();
        if (MaydoneNewPlan == null)
        {
            Notification.Instance.Show("Internal Error: Stupid Medet forgive him for his mistake, he is a human after all");
            return;
        }

        MaydoneNewPlan.Show(logos, userScenario);
    }

}
