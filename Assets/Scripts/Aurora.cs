using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aurora : MonoBehaviour
{
    private static Aurora _instance;
    private Aurora_NewUserScenario AuroraNewUserScenario;

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

    void Start()
    {
        TestLogos();
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
        NewScenario(logos);
    }

    public void NewScenario(AraDiscussion logos)
    {
        if (AuroraNewUserScenario == null)
        {
            Notification.Instance.Show("Internal Error: Stupid Medet forgive him for his mistake, he is a human after all");
        }

        AuroraNewUserScenario.Show(logos);
    }

}
