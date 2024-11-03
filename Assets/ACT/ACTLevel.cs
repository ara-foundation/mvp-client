using Lean.Gui;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ACTLevel: MonoBehaviour
{
    [SerializeField] private GameObject SaveObj;
    [SerializeField] private LeanButton Button;
    [SerializeField] private TextMeshProUGUI DefaultLabel;
    [SerializeField] private TextMeshProUGUI SelectedLabel;
    [SerializeField] private Text LevelLabel;
    [SerializeField] private LeanTooltipData TooltipData;

    private Action OnSave;
    private bool selected;
    private string projectName;
    private int level;

    public void Set(int level, bool selected, string projectName)
    {
        this.level = level;
        this.projectName = projectName;
        this.selected = selected;

        SaveObj.SetActive(false);
        if (selected)
        {
            // Selected button is active only by Save() operation
            Button.interactable = false;
        } else
        {
            // Other buttons will make scene switch
            Button.interactable = true;
        }
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

        SetTooltipData();
    }

    private void SetTooltipData()
    {
        if (selected)
        {
            if (SaveObj.activeSelf)
            {
                TooltipData.Text = $"Save the scene for '{projectName}'";
            } else
            {
                TooltipData.Text = $"Current level for '{projectName}'";
            }
        } else
        {
            TooltipData.Text = $"Click to enter to '{projectName}'";
        }
    }

    public void SetSave(Action onSave)
    {
        if (!selected)
        {
            return;
        }
        OnSave = onSave;
        SaveObj.SetActive(true);
        Button.interactable = true;
        SetTooltipData();
    }

    private void Saved()
    {
        SaveObj.SetActive(false);
        Button.interactable = false;
        SetTooltipData();
    }

    public void OnClick()
    {
        if (selected && OnSave != null)
        {
            Saved();
            OnSave();
        } else
        {
            ACTSession.Instance.RemoveLevelRange(level);
            StartCoroutine(RiseUp());
        }
    }

    IEnumerator RiseUp()
    {
        Global.Instance.ShowLoadingScene();

        yield return new WaitForSeconds(0.2f);

        // https://docs.unity3d.com/ScriptReference/AsyncOperation-allowSceneActivation.html
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}
