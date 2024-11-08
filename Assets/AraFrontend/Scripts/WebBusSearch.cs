using Lean.Gui;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using Button = UnityEngine.UI.Button;

public class WebBusSearch : MonoBehaviour
{
    [SerializeField] private LeanButton _searchBtn;
    [SerializeField] private Button _clearBtn;
    [SerializeField] private TMP_InputField _searchLabel;

    // To show the spinner
    public Func<string, Task<bool>> SearchCallback;
    public Action ClearCallback;

    // Start is called before the first frame update
    void Start()
    {
        ToggleBtns(enabled: false);
    }

    /// <summary>
    /// Called when clicked on the search button
    /// </summary>
    public void OnSearch()
    {
        if (SearchCallback == null)
        {
            return;
        }

        ToggleBtns(enabled: false);
        _searchLabel.interactable = false;

        var succeed = SearchCallback(_searchLabel.text);
        
        ToggleBtns(enabled: true);
        _searchLabel.interactable = true;
    }

    /// <summary>
    /// When clicked on the cancel button
    /// </summary>
    public void OnClear()
    {
        if (ClearCallback != null)
        {
            ClearCallback();
        }
        _searchLabel.text = "";
    }

    void ToggleBtns(bool enabled)
    {
        _clearBtn.gameObject.SetActive(enabled);
        _searchBtn.interactable = enabled;
    }

    public void OnSearchChange(string value)
    {
        if (string.IsNullOrEmpty(_searchLabel.text))
        {
            ToggleBtns(enabled: false);
            return;
        }

        ToggleBtns(enabled: true);
    }
}
