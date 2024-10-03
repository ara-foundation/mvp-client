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
    private bool _switchToAurora;

    // Start is called before the first frame update
    void Start()
    {
        _switchToAurora = false;
        IdeaTitle.text = "";
        IdeaContent.text = "";
    }

    public void SwitchToAurora(bool flag)
    {
        _switchToAurora = flag;
    }

    public async void OnPost()
    {
        if (!AraAuth.Instance.IsLoggedIn(AraAuth.Instance.UserParams))
        {
            AraAuth.Instance.RequireLogin();
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
        var createdLogos = await Post(ideaParams);
        PostButton.interactable = true;
        
        if (createdLogos == null) {
            return;
        }
        IdeaTitle.text = "";
        IdeaContent.text = "";

        if (!_switchToAurora)
        {
            _switchToAurora = false;
            Notification.Instance.Show("Idea was posted. Refreshing ideas to see result");
            await Logos.Instance.LoadIdeas();
        } else
        {
            Aurora.Instance.NewScenario(createdLogos);
        }
    }

    private async Task<AraDiscussion> Post(IdeaCreate ideaParams)
    {
        var body = JsonUtility.ToJson(ideaParams);
        string url = NetworkParams.AraActUrl + "/logos/idea";

        Tuple<long, string> res;
        try
        {
            res = await WebClient.Post(url, body);
        }
        catch (Exception ex)
        {
            Notification.Instance.Show($"Error: web client exception {ex.Message}");
            Debug.LogError(ex);
            return null;
        }
        if (res.Item1 != 200)
        {
            Notification.Instance.Show($"Error: {res.Item2}");
            return null;
        }

        AraDiscussion result;
        try
        {
            result = JsonConvert.DeserializeObject<AraDiscussion>(res.Item2);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Notification.Instance.Show($"Error: deserialization exception {e.Message}");
            return null;

        }

        return result;
    }

}
