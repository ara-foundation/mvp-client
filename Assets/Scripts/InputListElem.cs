using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputListElem : MonoBehaviour
{
    [SerializeField] private TMP_InputField InputContent;
    [SerializeField] private Button AddButton;
    [SerializeField] private Button RemoveButton;
    private InputList InputList = null;
    public bool IsAdd { get; private set; }

    public override string ToString()
    {
        return InputContent.text;
    }

    public string Content() {  return InputContent.text; }
    public void SetMode(InputList inputList, bool isAdd)
    {
        InputContent.text = "";
        IsAdd = isAdd;
        if (IsAdd)
        {
            AddButton.enabled = true;
            RemoveButton.enabled = false;
        } else
        {
            AddButton.enabled = false;
            RemoveButton.enabled = true;
        }
        InputList = inputList;
    }

    public void Set(InputList inputList, string content)
    {
        SetMode(inputList, false);
        InputContent.text = content;
    }

    public void OnRemove()
    {
        if (InputList.Remove(this))
        {
            Destroy(gameObject);
        }
    }

    public void OnAdd()
    {
        var content = InputContent.text;
        if (content.Length == 0)
        {
            Notification.Show("Error: please type the message");
            return;
        }

        InputList.Add(content);

        InputContent.text = "";
    }
}
