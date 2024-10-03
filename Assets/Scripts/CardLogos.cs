using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardLogos : MonoBehaviour
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

    // Ara Idea represented in the Ara Server
    private AraDiscussion serverContent;

    public void Show(AraDiscussion araDiscussion)
    {
        serverContent = araDiscussion;
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

    /** When a user wants to start new User Scenario
     */
    public void OnStart()
    {
        if (!AraAuth.Instance.IsLoggedIn(AraAuth.Instance.UserParams))
        {
            AraAuth.Instance.RequireLogin();
            return;
        }

        Aurora.Instance.NewScenario(serverContent);
    }
}
