using Lean.Gui;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IdeaCreate {
    public string token;
    public string title;
    public string content;
}


public class CardNewPost : MonoBehaviour
{
    [SerializeField] private TMP_InputField IdeaTitle;
    [SerializeField] private TMP_InputField IdeaContent;
    [SerializeField] private LeanButton PostButton;

    // Start is called before the first frame update
    void Start()
    {
        IdeaTitle.text = "";
        IdeaContent.text = "";
    }

    // Update is called once per frame
    public async void OnPost()
    {
        if (!AraAuth.Instance.IsLoggedIn(AraAuth.Instance.UserParams))
        {
            Notification.Instance.Show("Please Login First");
            return;
        }

        if (IdeaTitle.text.Length == 0)
        {
            Notification.Instance.Show("Write your idea's title");
            return;
        }

        if (IdeaContent.text.Length == 0)
        {
            Notification.Instance.Show("Write at least few sentences");
            return;
        }

        var ideaParams = new IdeaCreate
        {
            token = AraAuth.Instance.UserParams.token,
            title = IdeaTitle.text,
            content = IdeaContent.text
        };
        PostButton.interactable = false;
        await Post(ideaParams);
        PostButton.interactable = true;
        
        Notification.Instance.Show("Idea was posted. Refreshing ideas to see result");
        IdeaTitle.text = "";
        IdeaContent.text = "";

        await Logos.Instance.LoadIdeas();
    }

    private async Task Post(IdeaCreate ideaParams)
    {
        var body = JsonUtility.ToJson(ideaParams);
        string url = NetworkParams.AraActUrl + "/logos/idea";

        string res;
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

        CreateSessionToken result;
        try
        {
            result = JsonConvert.DeserializeObject<CreateSessionToken>(res);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Notification.Instance.Show($"Error: deserialization exception {e.Message}");
        }
    }
}
