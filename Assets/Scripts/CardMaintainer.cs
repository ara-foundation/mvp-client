using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardMaintainer: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Title;

    [SerializeField] private CardLogos logos;

    public CardAvatar Avatar;
    
    [SerializeField] private TextMeshProUGUI DisplayName;
    [SerializeField] private TextMeshProUGUI UserName;
    [SerializeField] private TextMeshProUGUI PublicationTime;
    [SerializeField] private TextMeshProUGUI UserBackground;
    [SerializeField] private TextMeshProUGUI Content;

    [SerializeField] private TextMeshProUGUI SharedAmount;
    [SerializeField] private TextMeshProUGUI MaydoneAmount;
    [SerializeField] private TextMeshProUGUI LikesAmount;
    [SerializeField] private TextMeshProUGUI ViewsAmount;

    private UserScenarioInServer userScenario;

    public void Show(UserScenarioInServer userScenario)
    {
        var content = "";
        for (int i = 0; i < userScenario.user_scenario_flow.Length; i++)
        {
            content += userScenario.user_scenario_flow[i].action;
            if (i != userScenario.user_scenario_flow.Length - 1)
            {
                content += " -> ";
            }
        }
        var createdAt = userScenario.forum_created_at;

        Debug.Log("Todo: Request the user's Display name by its id");

        Title.text = userScenario.title;
        PublicationTime.text = createdAt;
        Content.text = content;
        UserBackground.text = userScenario.context.background;
        SharedAmount.text = "0";
        MaydoneAmount.text = "0";
        LikesAmount.text = "0";
        ViewsAmount.text = "0";

        /// Show user information
        /*if (araDiscussion.relationships.user.attributes.avatarUrl != null )
        {
            Avatar.LoadAvatar(araDiscussion.relationships.user.attributes.avatarUrl);
        } else
        {
            Avatar.HideAvatar();
        }
        DisplayName.text = araDiscussion.relationships.user.attributes.displayName;*/
        Debug.Log("Todo: Load the user's avatar by its id");
        UserName.text = userScenario.forum_username;

        this.userScenario = userScenario;
    }

    public async void OnStart()
    {
        Debug.Log($"Scenario for {userScenario.logos_id} idea. Fetching...");
        var logos = await Logos.Instance.FetchIdea(userScenario.logos_id);
        if (logos == null)
        {
            Notification.Instance.Show("Error: failed to get logos idea from the server");
            return;
        }
        Debug.Log("Idea was fetched, open the new plan.");

        Maydone.Instance.NewPlan(logos, userScenario);
    }
}
