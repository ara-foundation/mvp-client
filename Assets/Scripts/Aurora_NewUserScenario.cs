using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SenseverScenarioRequest
{
    public string requester;
    public int id;
    public string content;
    public string token;
}

[Serializable]
public class SenseverScenarioResponse
{
    public bool correct;
    public UserScenario? answer;
}

[Serializable]
public class UserScenarioCreate
{
    public string token;    // authorization toke
    public int id; // logos id
    public UserScenario content;
}

public class Aurora_NewUserScenario : MonoBehaviour
{
    public bool SkipTutorial = false;
    [SerializeField] private Drawer_UserScenario Drawer;
    [SerializeField] private SenseverDialogue Dialogue;
    [SerializeField] private CardLogos PinnedLogos;
    [SerializeField] private Auroa_Tutorial Tutorial;
    [SerializeField] private Button PostButton;

    [SerializeField] private GameObject[] NewScenarioBackgrounds;

    private AraDiscussion logos;

    // Start is called before the first frame update
    void Start()
    {
        Dialogue.gameObject.SetActive(false);
        Drawer.gameObject.SetActive(false);
    }


    private void OnDisable()
    {
        Hide();
    }

    private void OnDialogueEnd(Auroa_Tutorial.TutorialStep showed)
    {
        if (showed == Auroa_Tutorial.TutorialStep.Exit) 
        {
            Tutorial.Hide();
        }
    }

    public void Hide()
    {
        Dialogue.gameObject.SetActive(false);
        Drawer.gameObject.SetActive(false);
        if (NewScenarioBackgrounds != null && NewScenarioBackgrounds.Length > 0)
        {
            for (int i = 0; i < NewScenarioBackgrounds.Length; i++)
            {
                if (NewScenarioBackgrounds[i].gameObject)
                {
                    NewScenarioBackgrounds[i].gameObject.SetActive(false);
                }
            }
        }
    } 

    public async void OnPost()
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

        var data = new UserScenarioCreate()
        {
            id = logos.id,
            token = AraAuth.Instance.UserParams.token,
            content = Drawer.Content(),
        };
        var body = JsonUtility.ToJson(data);
        PostButton.interactable = false;

        var url = NetworkParams.AraActUrl + "/aurora/user-scenario";

        Tuple<long, string> res;
        try
        {
            res = await WebClient.Post(url, body);
        }
        catch (Exception ex)
        {
            Notification.Instance.Show($"Error: web client exception {ex.Message}");
            Debug.LogError(ex);
            PostButton.interactable = true;
            return;
        }
        if (res.Item1 != 200)
        {
            Notification.Instance.Show($"Error: {res.Item2}");
            PostButton.interactable = true;
            return;
        }
        PostButton.interactable = true;
        Notification.Instance.Show("User Scenario was added, wait until someone starts a plan");
        Hide();
        Aurora.Instance.ShowUserScenarios();
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

    public void Show(AraDiscussion logos)
    {
        if (NewScenarioBackgrounds != null && NewScenarioBackgrounds.Length > 0)
        {
            for (int i = 0; i < NewScenarioBackgrounds.Length; i++)
            {
                NewScenarioBackgrounds[i].gameObject.SetActive(true);
            }
        }

        if (!SkipTutorial)
        {
            Drawer.gameObject.SetActive(false);
            Dialogue.gameObject.SetActive(true);
            Dialogue.StartTutorial(OnDialogueEnd, OnDialogueStart);
        }
        else
        {
            Drawer.gameObject.SetActive(true);
        }
        PinnedLogos.Show(logos);

        // load user scenario
        this.logos = logos;
        Drawer.SetReady(false);
        if (!AraAuth.Instance.IsLoggedIn(AraAuth.Instance.UserParams))
        {
            Drawer.SetTitle("Log in first");
            AraAuth.Instance.OnStatusChange += LoadScenario;
        }
        else
        {
            LoadScenario(true);
        }
    }

    async void LoadScenario(bool loggedIn)
    {
        if (!loggedIn)
        {
            return;
        }
        AraAuth.Instance.OnStatusChange -= LoadScenario;
        Drawer.SetTitle();

        var userScenario = await GenerateScenarioDraft(logos);
        if (userScenario != null)
        {
            Drawer.Show(logos, userScenario);
            Drawer.SetReady(true);
        }
        else
        {
            Drawer.gameObject.SetActive(false);
        }
    }

    async Task<UserScenario> GenerateScenarioDraft(AraDiscussion logos)
    {

        var reqBody = new SenseverScenarioRequest()
        {
            requester = "ahmetson",
            id = logos.id,
            content = logos.relationships.firstPost.attributes.contentHtml,
            token = AraAuth.Instance.UserParams.token,
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
