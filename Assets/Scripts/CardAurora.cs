using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class FlowStep {
    public int step;
    public string action;
    public string description;
}

[Serializable]
public class UserScenarioContext
{
    public string user;
    public string background;
    public string[] steps;
}

[Serializable]
public class UserScenarioProblem
{
    public string description;
    public string[] obstacles;
}

[Serializable]
public class UserScenario
{
    public string title;
    public UserScenarioContext context;
    public string[] goals;
    public UserScenarioProblem[] problems;
    public string[] user_motivations;
    public string[] personal_traits;
    public string[] relevant_habits_hobbies_beliefs;
    public FlowStep[] user_scenario_flow;
}

[Serializable]
public class UserScenarioInServer: UserScenario
{
    public int forum_discussion_id;
    public string forum_username;
    public int forum_user_id;
    public string forum_created_at;
    public int logos_id;
}

public class CardAurora : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI Title;

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
    }

    public void OnStart()
    {
        Debug.Log("Todo: switch to the Maydone scene now");
    }
}
