using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class ACTPart_controller : MonoBehaviour
{
    [SerializeField] public Canvas Canvas;
    [SerializeField] public GameObject Menu;
    [SerializeField] public Transform SplinePositionerContent;

    [Header("Project Name")]
    [SerializeField] public Transform ProjectNameContainer;
    [SerializeField] public TextMeshProUGUI ProjectNameLabel;
    [SerializeField] public ActivityState ProjectNameState;
    [SerializeField] public GameObject ProjectNameFieldContainer;
    [Space(20)]
    [Header("Tech Stack")]
    [SerializeField] public ActivityState TechStackMenuButton;
    [SerializeField] public LeanWindow TechStackWindow;
    [SerializeField] public Transform TechStackCameraTarget;
    [SerializeField] public TMP_InputField TechStackContent;
    [Space(20)]
    [Header("Budget")]
    [SerializeField] public TextMeshProUGUI BudgetMenuLabel;
    [SerializeField] public LeanWindow BudgetWindow;
    [SerializeField] public Transform BudgetCameraTarget;
    [SerializeField] public PieChart.ViitorCloud.PieChart BudgetPieChart;
    [Space(20)]
    [Header("Maintainer")]
    [SerializeField] public TextMeshProUGUI MaintainerMenuLabel;
    [SerializeField] public ActivityState MaintainerState;
    [SerializeField] public GameObject MaintainerInputFieldContainer;
    [SerializeField] public Transform MaintainerCameraTarget;

    /// Tasks are managed in the nested level so it's a part of ACTPart
}
