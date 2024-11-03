using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ACTPart_controller : MonoBehaviour
{
    private ACTPartModel _model;
    [SerializeField] public Canvas Canvas;
    [SerializeField] public GameObject HintReactor;
    [SerializeField] public GameObject Menu;
    [SerializeField] public Transform SplinePositionerContent;

    [Header("Parts or Tasks")]
    [SerializeField] private TextMeshProUGUI PartsAmount;

    [Header("Project Name")]
    [SerializeField] public Transform ProjectNameContainer;
    [SerializeField] private TextMeshProUGUI ProjectNameLabel;
    [SerializeField] public ActivityState ProjectNameState;
    [SerializeField] public GameObject ProjectNameFieldContainer;
    [Space(20)]
    [Header("Tech Stack")]
    [SerializeField] public ActivityState TechStackMenuButton;
    [SerializeField] public LeanWindow TechStackWindow;
    [SerializeField] public Transform TechStackCameraTarget;
    [SerializeField] private TMP_InputField TechStackContent;
    [Space(20)]
    [Header("Budget")]
    [SerializeField] private TextMeshProUGUI BudgetMenuLabel;
    [SerializeField] public LeanWindow BudgetWindow;
    [SerializeField] public Transform BudgetCameraTarget;
    
    [SerializeField] private TextMeshProUGUI BudgetMinLabel;
    [SerializeField] private TextMeshProUGUI BudgetMaxLabel;
    [SerializeField] private Slider BudgetSlider;
    [SerializeField] private TMP_InputField BudgetInput;

    private int availablePieIndex = 1;
    private int allocatedPieIndex = 2;
    private decimal maxAvailableBudget = 0;
    private readonly decimal minAllocatedPie = new decimal(0.1);
    [SerializeField] private PieChart.ViitorCloud.PieChart BudgetPieChart;
    [Space(20)]
    [Header("Maintainer")]
    [SerializeField] private TextMeshProUGUI MaintainerMenuLabel;
    [SerializeField] public ActivityState MaintainerState;
    [SerializeField] public GameObject MaintainerInputFieldContainer;
    [SerializeField] public Transform MaintainerCameraTarget;

    public ACTPartModel Model { get { return _model; } }

    void Start()
    {
        // Show a project name label, hide the project name editing field.
        ToggleProjectNameEditing(edit: false);
        ToggleMaintainerEditing(edit: false);
    }

    //
    // Tasks are managed in the nested level so it's a part of ACTPart
    //

    /// <summary>
    /// Switch between editing the project name and showing it as a label
    /// </summary>
    /// <param name="edit"></param>
    public void ToggleProjectNameEditing(bool edit)
    {
        ProjectNameState.gameObject.SetActive(!edit);
        ProjectNameFieldContainer.SetActive(edit);
    }

    public string ProjectName()
    {
        return ProjectNameLabel.text;
    }

    public string TechStack()
    {
        return TechStackContent.text;
    }

    public void ToggleMaintainerEditing(bool edit)
    {
        if (MaintainerMenuLabel != null)
        {
            MaintainerMenuLabel.gameObject.SetActive(!edit);
            MaintainerInputFieldContainer.SetActive(edit);
        }
    }

    public ACTPartModel SetProjectName(string projectName)
    {
        if (this._model.projectName == null)
        {
            this._model.projectName = "";
        }
        this._model.projectName = projectName;
        ProjectNameLabel.text = projectName;
        return this._model;
    }

    public ACTPartModel SetTechStack(string techStack)
    {
        this._model.techStack = techStack;
        TechStackContent.text = techStack;
        return this._model;
    }

    public void SetBudgetBox(decimal budget, decimal usedBudget)
    {
        var usedBudgetExceptThisModel = (usedBudget - _model.budget);
        maxAvailableBudget = (budget - usedBudgetExceptThisModel - (_model.usedBudget ?? 0));

        BudgetMinLabel.text = $"From $0 {{";
        BudgetMaxLabel.text = $"}} To ${maxAvailableBudget}";
        BudgetSlider.minValue = 0;
        BudgetSlider.maxValue = (float)maxAvailableBudget;
        bool generateInitialChart = BudgetSlider.value.Equals((float)_model.budget);
        SetBudgetChart(budget, usedBudgetExceptThisModel, generateInitialChart);
     
        // It will call the ChangeBudgetAllocation event which on it's own will draw the chart and set the fields
        if (!BudgetSlider.value.Equals((float)_model.budget)) {
            BudgetSlider.value = (float)_model.budget;
        }
    }

    public decimal BudgetSliderValue()
    {
        return (decimal)BudgetSlider.value;
    }

    public void ChangeBudgetAllocation(decimal budgetAllocation, bool updateInput, bool updateSlider)
    {
        if (budgetAllocation < minAllocatedPie)
        {
            budgetAllocation = minAllocatedPie;
        } else if (budgetAllocation > maxAvailableBudget)
        {
            budgetAllocation = maxAvailableBudget;
        }
        BudgetPieChart.Data[availablePieIndex] = maxAvailableBudget - budgetAllocation; // available
        BudgetPieChart.Data[allocatedPieIndex] = budgetAllocation;

        if (updateInput)
        {
            BudgetInput.text = budgetAllocation.ToString();
        }
        if (updateSlider)
        {
            BudgetSlider.value = (float)budgetAllocation;
        }

        BudgetPieChart.animationType = PieChart.ViitorCloud.PieChartMeshController.AnimationType.NoAnimation;
        BudgetPieChart.GenerateChart();
    }

    private void SetBudgetChart(decimal budget, decimal usedBudget, bool generateChart)
    {
        var allocatedPie = minAllocatedPie;
        if (_model.budget != 0)
        {
            allocatedPie = _model.budget;
        }
        // pie chart
        // total is entier budget
        if (usedBudget == 0)
        {
            BudgetPieChart.Data = new decimal[2];
            availablePieIndex = 0;
            allocatedPieIndex = 1;
        }
        else
        {
            // the part already used some money, we are adjusting it.
            if (_model.usedBudget > 0)
            {
                BudgetPieChart.Data = new decimal[4];
                BudgetPieChart.Data[1] = (decimal)_model.usedBudget;
                availablePieIndex = 2;
                allocatedPieIndex = 3;
            }
            else
            {
                BudgetPieChart.Data = new decimal[3];
                availablePieIndex = 1;
                allocatedPieIndex = 2;
            }

            BudgetPieChart.Data[0] = usedBudget; // used budget, the remaining is what is available to this part.

        }
        BudgetPieChart.Data[availablePieIndex] = maxAvailableBudget - allocatedPie; // available
        BudgetPieChart.Data[allocatedPieIndex] = allocatedPie; // as slider moves, move this value to along with previous pie

        // set slider
        // set min/max
        // set link between slider and value in edit
        BudgetPieChart.segments = BudgetPieChart.Data.Length;
        if (generateChart)
        {
            BudgetPieChart.animationType = PieChart.ViitorCloud.PieChartMeshController.AnimationType.Rotation;
            BudgetPieChart.GenerateChart();
        }
    }

    public ACTPartModel SetBudget(decimal budget)
    {
        if (this._model.budget == budget)
        {
            return null;
        }
        this._model.budget = budget;
        SetBudgetInternal(budget);
        return this._model;
    }

    public ACTPartModel SetMaintainerName(string maintainerName)
    {
        this._model.maintainer = maintainerName;
        SetMaintainerNameInternal(maintainerName);
        return this._model;
    }

    private void SetBudgetInternal(decimal budget)
    {
        if (BudgetMenuLabel != null)
        {
            BudgetMenuLabel.text = $"Budget: ${budget}";
        }

        // also for the pie chart
    }

    private void SetMaintainerNameInternal(string maintainerName)
    {
        if (MaintainerMenuLabel != null)
        {
            MaintainerMenuLabel.text = "Maintainer: " + maintainerName;
        }
    }

    public void SetData(ACTPartModel model)
    {
        if (_model != null)
        {
            return;
        }
        _model = model;
        if (model.childObjsId != null && model.childObjsId.Length > 0) {
            PartsAmount.text = $"Parts: {model.childObjsId.Length}";
        } else
        {
            PartsAmount.text = "Parts: 0";
        }
        if (string.IsNullOrEmpty(model.projectName))
        {
            ProjectNameLabel.text = "Part name";
        } else
        {
            ProjectNameLabel.text = model.projectName;
        }
        if (string.IsNullOrEmpty(model.techStack))
        {
            TechStackContent.text = "";
        }
        else
        {
            TechStackContent.text = model.techStack;
        }
        SetBudgetInternal(model.budget);
        if (string.IsNullOrEmpty(model.maintainer))
        {
            if (AraAuth.Instance != null && AraAuth.Instance.IsLoggedIn(AraAuth.Instance.UserParams))
            {
                SetMaintainerNameInternal(AraAuth.Instance.UserParams.loginParams.username);
            }
            else
            {
                SetMaintainerNameInternal("none");
            }
        }
        else
        {
            SetMaintainerNameInternal(model.maintainer);
        }
    }
}
