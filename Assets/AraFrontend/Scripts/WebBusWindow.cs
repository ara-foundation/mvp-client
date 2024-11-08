using Lean.Gui;
using Newtonsoft.Json;
using Rundo.RuntimeEditor.Behaviours.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

[Serializable]
public class DIOSTransfer : Project
{
    public string version_update_time;
    public string[] inputs;        // Equivalent of DIOSData.Type enum
    public string[] outputs;
}


public class WebBusWindow : MonoBehaviour
{
    private static WebBusWindow _instance;

    [SerializeField] private LeanWindow LeanWindow;
    [SerializeField] private TMP_Text Title;
    [SerializeField] private WebBusSearch WebBusSearch;
    [SerializeField] private SelectedItem SelectedInputItem;
    [SerializeField] private SelectedItem SelectedOutputItem;
    [SerializeField] private GameObject DIOSTransferPrefab;
    [SerializeField] private LeanWindow TransferContentWindow;
    [SerializeField] private Content TransferContent;
    [SerializeField] private LeanWindow TransferLoadingWindow;

    private string searched = "";
    private DIOSTransfer[] DIOSTransfers = new DIOSTransfer[0];

    public static WebBusWindow Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<WebBusWindow>();
            }
            return _instance;
        }
    }

    private void UpdateTitle()
    {
        var transferParams = TransferParams();
        if (transferParams.inputs.Length > 0 && transferParams.outputs.Length > 0) {
            Title.text = $"Transfers {transferParams.inputs[0]} -> {transferParams.outputs[0]} ({DIOSTransfers.Length})";
        } else if (transferParams.inputs.Length > 0)
        {
            Title.text = $"Transfers that receives {transferParams.inputs[0]} ({DIOSTransfers.Length})";
        } else if (transferParams.outputs.Length > 0)
        {
            Title.text = $"Transfers that outputs {transferParams.outputs[0]} ({DIOSTransfers.Length})";
        } else
        {
            Title.text = $"Transfers ({DIOSTransfers.Length})";
        }
    }

    private void Awake()
    {
        SelectedOutputItem.ShowDefault();
        SelectedInputItem.ShowDefault();
        WebBusSearch.SearchCallback = SearchFor;
        WebBusSearch.ClearCallback = ClearSearch;
        TransferContent.Clear();
        UpdateTitle();
    }

    private DIOSTransfer TransferParams()
    {
        DIOSTransfer transferParams = new()
        {
            inputs = new string[] { },
            outputs = new string[] { }
        };
        if (SelectedInputItem.IsShowing())
        {
            transferParams.inputs = new string[1] { SelectedInputItem.DIOSTypeString() };
        }
        if (SelectedOutputItem.IsShowing())
        {
            transferParams.outputs = new string[1] { SelectedOutputItem.DIOSTypeString() };
        }

        return transferParams;
    }

    public async Task<bool> SearchFor(string value)
    {
        searched = value;
        TransferLoadingWindow.TurnOn();
        
        DIOSTransfers = await FetchDiosTransfers(value);
        TransferContent.Clear();
        if (DIOSTransfers.Length > 0)
        {
            foreach (var transfer in DIOSTransfers)
            {
                var card = TransferContent.Add<CardDIOSTransfer>(DIOSTransferPrefab);
                card.Show(transfer);
            }
        }
        UpdateTitle();
        TransferContentWindow.TurnOn();
        return DIOSTransfers.Length > 0;
    }

    public async Task<bool> SearchFor()
    {
        return await SearchFor(searched);
    }

    public void ClearSearch() {
        if (string.IsNullOrEmpty(searched))
        {
            return;
        }

        TransferContent.Clear();
        searched = string.Empty;
    }

    private async Task<DIOSTransfer[]> FetchDiosTransfers(string value)
    {
        DIOSTransfer[] incorrectResult = new DIOSTransfer[] { };

        string url = NetworkParams.AraActUrl + "/dios/transfers?search=" + value;

        string body = JsonConvert.SerializeObject(TransferParams());

        Tuple<long, string> res;
        try
        {
            res = await WebClient.Post(url, body);
        }
        catch (Exception ex)
        {
            Notification.Instance.Show($"Error to fetch transfers: client exception {ex.Message}");
            Debug.LogError(ex);
            return incorrectResult;
        }
        if (res.Item1 != 200)
        {
            Notification.Instance.Show($"Error to fetch transfers http code ({res.Item1}): {res.Item2}");
            return incorrectResult;
        }

        DIOSTransfer[] result;
        try
        {
            result = JsonConvert.DeserializeObject<DIOSTransfer[]>(res.Item2);
        }
        catch (Exception e)
        {
            Debug.LogError(e + " for " + res);
            return incorrectResult;
        }
        return result;
    }

    public void ShowWindow()
    {
        ShowWindow(loadData: false);
    }

    public async void ShowWindow(bool loadData)
    {
        if (!LeanWindow.On)
        {
            LeanWindow.TurnOn();
        }
        if (!TransferContentWindow.On)
        {
            TransferContentWindow.TurnOn();
        }

        if (loadData)
        {
            UpdateTitle();
            await SearchFor();
        }
    }

    public void HideWindow()
    {
        LeanWindow.TurnOff();
    }


    public async void ShowInput(ProjectItemMetaData metaData)
    {
        ShowWindow();
        SelectedInputItem.Show(metaData, btnEnabled: true);
        if (!SelectedOutputItem.IsShowing()) {
            SelectedOutputItem.ShowDefault();
        }
        UpdateTitle();
        await SearchFor();
    }

    public async void ShowOutput(ProjectItemMetaData metaData)
    {
        ShowWindow();
        SelectedOutputItem.Show(metaData, btnEnabled: true);
        if (!SelectedInputItem.IsShowing())
        {
            SelectedInputItem.ShowDefault();
        }
        UpdateTitle();
        await SearchFor();
    }

    public async void OnCancelInput()
    {
        AraFrontend.Instance.OnDeselectInputItem(SelectedInputItem.Data);
        SelectedInputItem.ShowDefault();
        UpdateTitle();
        await SearchFor();
    }

    public async void OnCancelOutput()
    {
        AraFrontend.Instance.OnDeselectOutputItem(SelectedOutputItem.Data);
        SelectedOutputItem.ShowDefault();
        UpdateTitle();
        await SearchFor();
    }
}
