using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlowStep {
    public int step;
    public string action;
    public string description;
}

public class UserScenarioContext
{
    public string user;
    public string background;
    public string[] steps;
}

public class UserScenarioProblem
{
    public string description;
    public string[] obstacles;
}

public class UserScenario {
    public string title;
    public UserScenarioContext context;
    public string[] goals;
    public UserScenarioProblem[] problems;
    public string[] user_motivations;
    public string[] personal_traits;
    public string[] relevant_habits_hobbies_beliefs;
    public FlowStep[] user_scenario_flow;
}

public class CardAurora : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI Title;

    public CardAvatar Avatar;
    
    [SerializeField] private TextMeshProUGUI DisplayName;
    [SerializeField] private TextMeshProUGUI UserName;
    [SerializeField] private TextMeshProUGUI PublicationTime;
    [SerializeField] private TextMeshProUGUI Content;

    [SerializeField] private TextMeshProUGUI SharedAmount;
    [SerializeField] private TextMeshProUGUI CommentsAmount;
    [SerializeField] private TextMeshProUGUI AurorasAmount;
    [SerializeField] private TextMeshProUGUI LikesAmount;
    [SerializeField] private TextMeshProUGUI ViewsAmount;

    public void Show(AraDiscussion araDiscussion)
    {
        var content = araDiscussion.relationships.firstPost.attributes.contentHtml; 
        if (content != null && content.Length > 300) {
            content = content.Substring(0, 300);
        }
        var createdAt = araDiscussion.relationships.firstPost.attributes.createdAt.Split(":")[0];

        Title.text = araDiscussion.attributes.title;
        PublicationTime.text = createdAt;
        Content.text = content;
        SharedAmount.text = "0";
        AurorasAmount.text = "0";
        LikesAmount.text = "0";
        ViewsAmount.text = "0";
        CommentsAmount.text = araDiscussion.attributes.commentCount.ToString();

        /// Show user information
        if (araDiscussion.relationships.user.attributes.avatarUrl != null )
        {
            Avatar.LoadAvatar(araDiscussion.relationships.user.attributes.avatarUrl);
        } else
        {
            Avatar.HideAvatar();
        }
        DisplayName.text = araDiscussion.relationships.user.attributes.displayName;
        UserName.text = araDiscussion.relationships.user.attributes.username;
    }
}
