using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class SenseverScenarioRequest
{
    public string requester;
    public int id;
    public string content;
}

public class SenseverScenarioResponse
{
    public bool correct;
    public UserScenario? answer;
}

public class Aurora_NewUserScenario : MonoBehaviour
{
    public bool SkipTutorial = false;
    [SerializeField] private Drawer_UserScenario Drawer;
    [SerializeField] private SenseverDialogue Dialogue;
    [SerializeField] private CardLogos PinnedLogos;
    [SerializeField] private Auroa_Tutorial Tutorial;

    // Start is called before the first frame update
    void Start()
    {
        Dialogue.gameObject.SetActive(false);
        Drawer.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Dialogue.gameObject.SetActive(false);
        Drawer.gameObject.SetActive(false);
    }

    private void OnDialogueEnd(Auroa_Tutorial.TutorialStep showed)
    {
        if (showed == Auroa_Tutorial.TutorialStep.Exit) 
        {
            Tutorial.Hide();
        }
    }

    public void OnPost()
    {
        var error = Drawer.Validate();
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

        var content = Drawer.Content();
        var body = JsonUtility.ToJson(content);
    }

    private void OnDialogueStart(Auroa_Tutorial.TutorialStep started)
    {
        if (started == Auroa_Tutorial.TutorialStep.Idea)
        {
            Tutorial.ShowFog();
            Tutorial.ShowIdeaArea();
        } else if (started == Auroa_Tutorial.TutorialStep.Exit)
        {
            Tutorial.ShowExitArea();
        } else if (started == Auroa_Tutorial.TutorialStep.Enjoy)
        {
            Tutorial.Hide();
        } else if (started == Auroa_Tutorial.TutorialStep.None)
        {
            Dialogue.gameObject.SetActive(false);
            Drawer.gameObject.SetActive(true);
        }
    }

    public async void Show(AraDiscussion logos)
    {
        if (!SkipTutorial)
        {
            Drawer.gameObject.SetActive(false);
            Dialogue.gameObject.SetActive(true);
            Dialogue.StartTutorial(OnDialogueEnd, OnDialogueStart);
        } else
        {
            Drawer.gameObject.SetActive(true);
        }
        PinnedLogos.Show(logos);

        // load user scenario
        var userScenario = await GenerateScenarioDraft(logos);
        if (userScenario != null)
        {
            Drawer.Show(logos, userScenario);
            Drawer.SetReady(true);
        } else
        {
            Drawer.SetReady(false);
            Drawer.gameObject.SetActive(false);
        }
        // post scenario
    }

    async Task<UserScenario> GenerateScenarioDraft(AraDiscussion logos)
    {
        var reqBody = new SenseverScenarioRequest()
        {
            requester = "ahmetson",
            id = logos.id,
            content = logos.relationships.firstPost.attributes.contentHtml
        };

        var url = NetworkParams.SenseverUrl + "/scenario-draft";
        var body = JsonUtility.ToJson(reqBody);

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

        SenseverScenarioResponse result;
        try
        {
            result = JsonConvert.DeserializeObject<SenseverScenarioResponse>(res.Item2);
            if (!result.correct)
            {
                Notification.Instance.Show("Sensever server returned unrecognized data. So sorry for my mistake");
                return null;
            }

            return result.answer as UserScenario;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Notification.Instance.Show($"Error: deserialization exception {e.Message}");
            return null;
        }
    }
}
