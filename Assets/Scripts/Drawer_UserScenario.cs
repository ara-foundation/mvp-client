using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Drawer_UserScenario : MonoBehaviour
{
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
    [SerializeField] private TMP_InputField Describe;
    [SerializeField] private InputList Problems;
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

    void Start()
    {
        contentReady.Set(false);
    }

    private void OnDisable()
    {
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
