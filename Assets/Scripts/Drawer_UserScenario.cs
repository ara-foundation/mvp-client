using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Drawer_UserScenario : MonoBehaviour
{
    private AraDiscussion logos;
    private UserScenario draft;

    [Header("General")]
    [SerializeField] private LeanToggle contentReady;
    [Space(20)]
    static public string DefaultTitle = "Generating User Scenario...";
    [SerializeField] private TextMeshProUGUI Title;
    [SerializeField] private TextMeshProUGUI Author;
    [Header("Context")]
    [SerializeField] private TMP_InputField UserName;
    [SerializeField] private TMP_InputField UserBackground;
    [SerializeField] private InputList Steps;
    [SerializeField] private InputList Goals;
    [SerializeField] private InputList Habits;
    [Space(10)]
    [Header("Problems")]
    [SerializeField] private List<UserScenario_Problem> Problems;
    [SerializeField] private Transform ProblemsContent;
    [SerializeField] private GameObject ProblemPrefab;
    [Space(10)]
    [Header("Motivation")]
    [SerializeField] private InputList Motivations;
    [Space(10)]
    [Header("User Flow")]
    [SerializeField] private InputList Flow;

    public void SetReady(bool ready)
    {
        contentReady.Set(ready);
    }

    public void Show(AraDiscussion logos, UserScenario draft)
    {
        this.logos = logos;
        this.draft = draft;

        Title.text = draft.title;
        // Context
        UserName.text = draft.context.user;
        UserBackground.text = draft.context.background;
        for (int i = 0; i < draft.context.steps.Length; i++)
        {
            Steps.Add(draft.context.steps[i]);
        }
        for (int i = 0; i < draft.goals.Length; i++)
        {
            Goals.Add(draft.goals[i]);
        }
        for (int i = 0; i < draft.relevant_habits_hobbies_beliefs.Length; i++)
        {
            Habits.Add(draft.relevant_habits_hobbies_beliefs[i]);
        }
        // Problems
        ClearContent(ProblemsContent);
        Problems = new List<UserScenario_Problem>();
        for (int i = 0; i < draft.problems.Length; i++)
        {
            var problem = Instantiate(ProblemPrefab, ProblemsContent);
            var problemScript = problem.GetComponent<UserScenario_Problem>();
            problemScript.Show(draft.problems[i]);
            Problems.Add(problemScript);

        }
        // Motivation
        for (int i = 0; i < draft.user_motivations.Length; i++)
        {
            Motivations.Add(draft.user_motivations[i]);
        }
        // Flow
        for (int i = 0; i < draft.user_scenario_flow.Length; i++)
        {
            var flow = draft.user_scenario_flow[i];
            Flow.Add(flow.action + ": " + flow.description);
        }
    }

    public void Hide()
    {
        this.logos = null;
        this.draft = null;

        Title.text = DefaultTitle;
        // Context
        UserName.text = "";
        UserBackground.text = "";
        Steps.ClearContent();
        Goals.ClearContent();
        Habits.ClearContent();
        // Problems
        ClearContent(ProblemsContent);
        Problems = new List<UserScenario_Problem>();
        Motivations.ClearContent();
        Flow.ClearContent();
    }

    private void ClearContent(Transform trans)
    {
        foreach (Transform child in trans)
        {
            Destroy(child.gameObject);
        };
    }

    void Start()
    {
        Hide();
        contentReady.Set(false);
    }

    private void OnDisable()
    {
        Hide();
        contentReady.Set(false);
    }

    public void OnLoading()
    {
        Title.text = DefaultTitle;
        if (AraAuth.Instance.IsLoggedIn(AraAuth.Instance.UserParams))
        {
            Author.text = "@" + AraAuth.Instance.UserParams.loginParams.username;
        } else
        {
            Author.text = "Anonymous";
        }
    }

    public void OnReady()
    {
        Title.text = "Here is the user scenario";
    }
}
