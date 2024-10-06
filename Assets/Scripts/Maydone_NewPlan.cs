using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlanCreate
{
    public string token;    // authorization toke
    public Plan content;
}

public class Maydone_NewPlan : MonoBehaviour
{
    [Header("Welcome page")]
    [SerializeField] public Plan Plan;
    [SerializeField] private CardLogos WelcomeLogos;
    [Space(20)]
    [Header("Form")]
    [SerializeField] private CardLogos FormLogos;
    [Space(10)]
    [SerializeField] private Button DeployButton;
    [SerializeField] private GameObject DeployingSpinner;
    [SerializeField] private Button LastReturnButton;

    private AraDiscussion logos;

    public void OnChangeProjectName(string value)
    {
        if (Plan != null)
        {
            Plan.project_name = value;
        }
    }

    public void OnChangeTechStack(string value)
    {
        if (Plan != null)
        {
            Plan.tech_stack = value;
        }
    }

    public void OnChangeCost(string value)
    {
        if (Plan != null)
        {
            Int32.TryParse(value, out Plan.cost_usd);
        }
    }

    public void OnChangeDeadline(string value)
    {
        if (Plan != null)
        {
            Int32.TryParse(value, out Plan.duration);
        }
    }

    public void OnChangeSourceCodeUrl(string value)
    {
        if (Plan != null)
        {
            Plan.source_code_url = value;
        }
    }

    public void OnChangeTestUrl(string value)
    {
        if (Plan != null)
        {
            Plan.test_url = value;
        }
    }

    public void OnChangeTokenName(string value)
    {
        if (Plan != null)
        {
            Plan.token_name = value;
        }
    }

    public void OnChangeTokenSymbol(string value)
    {
        if (Plan != null)
        {
            Plan.token_symbol = value;
        }
    }

    public void OnChangeTokenMaxSupply(string value)
    {
        if (Plan != null)
        {
            Plan.token_max_supply = value;
        }
    }

    public void OnChangeSnaghaWelcome(string value)
    {
        if (Plan != null)
        {
            Plan.sangha_welcome = value;
        }
    }

    public void Hide()
    {

        DeployButton.interactable = true;
        DeployingSpinner.SetActive(false);
        LastReturnButton.interactable = true;

        Debug.Log("Hide the new plan, reset the plan");
        Plan = new Plan();
        logos = null;
    } 

    public async void OnDeploy()
    {
        var error = Plan.Validate();
        if (!string.IsNullOrEmpty(error))
        {
            Notification.Instance.Show("Error: " + error);
            return;
        }
        if (!AraAuth.Instance.IsLoggedIn(AraAuth.Instance.UserParams))
        {
            AraAuth.Instance.RequireLogin();
            return;
        }

        Notification.Instance.Show("Todo send to the server our maydone addition");
        return;

        var data = new PlanCreate()
        {
            content = Plan,
            token = AraAuth.Instance.UserParams.token,
        };
        var body = JsonUtility.ToJson(data);

        var url = NetworkParams.AraActUrl + "/maydone/plan";

        Tuple<long, string> res;
        try
        {
            res = await WebClient.Post(url, body);
        }
        catch (Exception ex)
        {
            Notification.Instance.Show($"Error: web client exception {ex.Message}");
            Debug.LogError(ex);
            return;
        }
        if (res.Item1 != 200)
        {
            Notification.Instance.Show($"Error: {res.Item2}");
            return;
        }
        Notification.Instance.Show("Plan was added, wait until someone starts investing. :)");
        Hide();
        await Maydone.Instance.ShowPlans();
    }


    public void Show(AraDiscussion logos, int userScenarioId)
    {
        // First, show the Warning
        // Then, show the Welcome:
        WelcomeLogos.Show(logos);
        FormLogos.Show(logos);

        Debug.Log("Logos id " + logos.id);

        Plan = new Plan
        {
            user_scenario_id = userScenarioId,
            logos_id = logos.id
        };
        if (AraAuth.Instance.IsLoggedIn(AraAuth.Instance.UserParams))
        {
            AttachLeader(true);
        } else
        {
            AraAuth.Instance.OnStatusChange += AttachLeader;
        }
    }

    private void AttachLeader(bool loggedIn)
    {
        if (!loggedIn)
        {
            return;
        }
        AraAuth.Instance.OnStatusChange -= AttachLeader;
        Plan.leader_username = AraAuth.Instance.UserParams.loginParams.username;
        Plan.leader_user_id = AraAuth.Instance.UserParams.loginParams.user_id;
    }



}
