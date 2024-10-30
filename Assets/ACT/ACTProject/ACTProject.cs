using Lean.Gui;
using Nethereum.Web3;
using RTS_Cam;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ACTProject : MonoBehaviour, IStateReactor
{
    [SerializeField] public ActivityState ActivityState;
    [SerializeField] private GameObject Menu;

    [Space(20)]
    [Header("UI Section")]
    [SerializeField] private TextMeshProUGUI ProjectNameLabel;
    [SerializeField] private TextMeshProUGUI PartsLabel;
    [SerializeField] private LeanTooltipData[] TechStackTooltipData;
    [SerializeField] private TextMeshProUGUI BudgetMenuLabel;
    [SerializeField] private TextMeshProUGUI MaintainerMenuLabel;
    [SerializeField] private TextMeshProUGUI TasksMenuLabel;
    [SerializeField] private TextMeshProUGUI ProgressLabel;

    private ActWithProjectAndPlan actWithProject;

    // Start is called before the first frame update
    void Start()
    {
        ActivityState.SetActivityGroup(ACTProjects.Instance.ActivityGroup);
        Menu.SetActive(false);
    }

    private void OnDestroy()
    {
        Menu.SetActive(false);
    }

    public void Select(bool enabled)
    {
        Menu.SetActive(enabled);
    }

    public void Focus(bool enabled)
    {
        if (!enabled)
        {
            return;
        }

        StartCoroutine(DiveInto());
    }

    IEnumerator DiveInto()
    {
        Debug.Log($"Double clicked, lets dive into {System.DateTime.Now}");

        ACTSession.Instance.SetFirstLevel(actWithProject);
        ACTProjects.Instance.LoadingSceneModal.Set(true);

        // https://docs.unity3d.com/ScriptReference/AsyncOperation-allowSceneActivation.html
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        asyncOperation.allowSceneActivation = true;

        var rts = ACTProjects.Instance.ACTProjectsCamera.gameObject.GetComponent<RTS_Camera>();
        
        var zoomIn = rts.targetOffset.z + 1;
        rts.targetOffset = new UnityEngine.Vector3(rts.targetOffset.x, rts.targetOffset.y, zoomIn);
        yield return null;
    }

    public void Highlight(bool enabled)
    {
    }

    public void Clear()
    {
    }

    public void Show(ActWithProjectAndPlan actWithProject)
    {
        this.actWithProject = actWithProject;

        ProjectNameLabel.text = actWithProject.project_v1[0].project_name;
        PartsLabel.text = "Parts: " + CountParts();
        SetTechStackTooltip();
        BudgetMenuLabel.text = "Budget: $" + GetBudget();
        MaintainerMenuLabel.text = "Maintainer: " + Maintainer();
        TasksMenuLabel.text = "Tasks: " + CountTasks();
        SetProgressBar();
    }

    private void SetProgressBar()
    {
        Debug.LogWarning("Todo! Progress bar is implemented after tasks");
        ProgressLabel.text = "0%";
    }

    private int CountTasks()
    {
        Debug.LogWarning("Todo! Task is not enabled yet. We will do it after setting everything with the server and levels");
        return 0;
    }

    private string Maintainer()
    {
        if (actWithProject.project_v1[0].leader != null)
        {
            return actWithProject.project_v1[0].leader.username;
        }
        return "<i>unknown</i>";
    }

    private decimal GetBudget()
    {
        if (actWithProject != null && actWithProject.plan != null && actWithProject.plan.Length > 0)
        {
            return Web3.Convert.FromWei(BigInteger.Parse(actWithProject.plan[0].cost_usd));
        }

        return 0;
    }

    private void SetTechStackTooltip()
    {
        for (int i = 0; i<TechStackTooltipData.Length; i++)
        {
            TechStackTooltipData[i].Text = this.actWithProject.tech_stack;
        }
    }

    private int CountParts()
    {
        if (actWithProject == null)
        {
            return 0;
        }
        return actWithProject.parts_amount ?? 0;
    }

}
