using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class CardDIOSTransfer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Title;
    [SerializeField] private TextMeshProUGUI UserName;
    [SerializeField] private TextMeshProUGUI PublicationTime;
    [Space(10)]
    [Header("DIOS")]
    [SerializeField] private TextMeshProUGUI Inputs;
    [SerializeField] private TextMeshProUGUI Outputs;
    [SerializeField] private TextMeshProUGUI IOFullContent;

    [Space(10)]
    [Header("Footer")]
    [SerializeField] private TextMeshProUGUI Version;
    [SerializeField] private TextMeshProUGUI Chat;
    [SerializeField] private TextMeshProUGUI Price;
    [SerializeField] private TextMeshProUGUI Speed;
    [SerializeField] private TextMeshProUGUI Likes;

    // Ara Idea represented in the Ara Server
    private DIOSTransfer serverContent;

    public Func<DIOSTransfer, bool> UseCallback;

    public void Show(DIOSTransfer diosTransfer)
    {
        serverContent = diosTransfer;
        Title.text = diosTransfer.project_name;
        if (diosTransfer.leader != null)
        {
            UserName.text = "@" + diosTransfer.leader.username;
        } else
        {
            UserName.text = "noname";
        }
        if (diosTransfer.version_update_time != null)
        {
            PublicationTime.text = diosTransfer.version_update_time;
        } else
        {
            PublicationTime.text = "08/11/2024";
        }

        Inputs.text = diosTransfer.inputs[0];
        if (diosTransfer.inputs.Length > 1 )
        {
            Inputs.text += "...";
        }

        Outputs.text = diosTransfer.outputs[0];
        if (diosTransfer.outputs.Length > 1)
        {
            Outputs.text += "...";
        }

        var lastInputIndex = diosTransfer.inputs.Length - 1;
        var lastOutputIndex = diosTransfer.outputs.Length - 1;
        var content = "Inputs: ";
        for (int i = 0; i <= lastInputIndex; i++)
        {
            content += diosTransfer.inputs[i];
            if (i != lastInputIndex)
            {
                content += ", ";
            }
        }
        content += $"{Environment.NewLine}Outputs: ";
        for (int i = 0; i <= lastOutputIndex; i++)
        {
            content += diosTransfer.outputs[i];
            if (i != lastOutputIndex)
            {
                content += ", ";
            }
        }

        IOFullContent.text = content;
    }

    public void OnUse()
    {
        var used = UseCallback(serverContent);
    }
}
