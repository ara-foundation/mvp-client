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
        Debug.LogWarning("There is no UserScenarioProblem management such as on the InputList to add or remove problems");
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
            if (child.gameObject)
            {
                Destroy(child.gameObject);
            }
        };
    }

    void Start()
    {
        Hide();
        contentReady.Set(false);
    }

    private void OnEnable()
    {
        AraAuth.Instance.OnStatusChange += OnAuthStatusChange;
    }

    private void OnDisable()
    {
        SetTitle();
        Hide();
        contentReady.Set(false);
        AraAuth.Instance.OnStatusChange -= OnAuthStatusChange;
    }

    private void OnAuthStatusChange(bool loggedIn)
    {
        if (!contentReady.On)
        {
            return;
        }
        if (loggedIn)
        {
            Author.text = "@" + AraAuth.Instance.UserParams.loginParams.username;
        }
        else
        {
            Author.text = "Anonymous";
        }
    }

    public void OnLoading()
    {
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
    }

    public void SetTitle(string title)
    {
        Title.text = title;
    }

    public void SetTitle()
    {
        Title.text = DefaultTitle;
    }

    /// <summary>
    /// Returns an error message if there is any
    /// </summary>
    /// <returns></returns>
    public string Validate()
    {
        if (logos == null)
        {
            return "No binded Logos";
        }

        if (string.IsNullOrEmpty(Title.text))
        {
            return "Title is empty";
        }
        if (string.IsNullOrEmpty(UserName.text))
        {
            return "Username is empty";
        }
        if (string.IsNullOrEmpty(UserBackground.text))
        {
            return "User background is empty";
        }
        if (Steps.IsEmpty())
        {
            return "No user steps";
        }
        if (Goals.IsEmpty())
        {
            return "No user goals";
        }
        if (Habits.IsEmpty())
        {
            return "Empty user habits";
        }
        if (Problems.Count == 0)
        {
            return "No user's problems defined";
        }
        for (int i = 0; i < Problems.Count; i++)
        {
            var problem = Problems[i];
            if (problem.IsEmpty())
            {
                return $"Problem {i+1} has empty field";
            }
        }
        if (Motivations.IsEmpty())
        {
            return "No motivation defined";
        }
        if (Flow.IsEmpty())
        {
            return "User Flow is not defined";
        }
        var flow = Flow.Elements();
        for (int i = 0; i < flow.Length; i++)
        {
            var flowParts = flow[i].Split(":");
            if (flowParts.Length != 2)
            {
                return $"Flow format error for {i+1}. Flow format must have action and description separation by ':'";
            }
        }

        return null;
    }

    public UserScenario Content()
    {
        draft.title = Title.text;
        // Context
        draft.context.user = UserName.text;
        draft.context.background = UserBackground.text;
        var steps = Steps.Elements();
        draft.context.steps = new string[steps.Length];
        for (int i = 0; i < steps.Length; i++)
        {
            draft.context.steps[i] = steps[i];
        }
        var goals = Goals.Elements();
        draft.goals = new string[goals.Length];

        for (int i = 0; i < goals.Length; i++)
        {
            draft.goals[i] = goals[i];
        }
        var habits = Habits.Elements();
        draft.relevant_habits_hobbies_beliefs = new string[habits.Length];
        for (int i = 0; i < habits.Length; i++)
        {
            draft.relevant_habits_hobbies_beliefs[i] = habits[i];
        }
        // Problems
        draft.problems = new UserScenarioProblem[Problems.Count];
        for (int i = 0; i < Problems.Count; i++)
        {
            draft.problems[i] = Problems[i].Content();
        }
        // Motivation
        var motivations = Motivations.Elements();
        draft.user_motivations= new string[motivations.Length];
        for (int i = 0; i < motivations.Length; i++)
        {
            draft.user_motivations[i] = motivations[i];
        }
        // Flow
        var flow = Flow.Elements();
        draft.user_scenario_flow = new FlowStep[flow.Length];
        for (int i = 0; i < flow.Length; i++)
        {
            var flowParts = flow[i].Split(":");

            var flowStep = new FlowStep()
            {
                    step = 1,
                    action = flowParts[0],
                    description = flowParts[1],
            };
            draft.user_scenario_flow[i] = flowStep;
        }

        return draft;
    }
}
