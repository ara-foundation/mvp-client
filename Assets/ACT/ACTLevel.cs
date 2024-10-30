using Lean.Gui;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ACTLevel: MonoBehaviour
{
    [SerializeField] private LeanButton Button;
    [SerializeField] private TextMeshProUGUI DefaultLabel;
    [SerializeField] private TextMeshProUGUI SelectedLabel;
    [SerializeField] private Text LevelLabel;
    [SerializeField] private LeanTooltipData TooltipData;

    public void Set(int level, bool selected, string projectName)
    {
        Debug.Log($"ACTLevel was set as {selected}");

        Button.interactable = selected;
        SelectedLabel.gameObject.SetActive(selected);
        DefaultLabel.gameObject.SetActive(!selected);

        LevelLabel.text = level.ToString();

        if (selected)
        {
            SelectedLabel.text = projectName;
        }
        else
        {
            DefaultLabel.text = projectName;
        }

        if (selected)
        {
            TooltipData.Text = $"Current level for '{projectName}'";
        }
        else
        {
            TooltipData.Text = $"Click to enter to '{projectName}'";
        }
    }


    public void OnClick()
    {
        Debug.Log("Clicked to the level");
    }
}
