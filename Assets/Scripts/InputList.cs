using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputList : MonoBehaviour
{
    [SerializeField] private GameObject InputListElemPrefab;
    [SerializeField] private Transform AddedContent;
    [SerializeField] private InputListElem StickedInputElem;
    private List<InputListElem> AddedListElems = new List<InputListElem>();

    // Start is called before the first frame update
    void OnEnable()
    {
        StickedInputElem.SetMode(this, true);
        ClearContent();
    }

    public void ClearContent()
    {
        foreach (Transform child in AddedContent)
        {
            Destroy(child.gameObject);
        };
    }


    public bool Remove(InputListElem inputListElem)
    {
        var removed = AddedListElems.Remove(inputListElem);
        if (removed)
        {
            Notification.Instance.Show("Removed element");
        } else
        {
            Notification.Instance.Show("Failed to remove the element");
        }
        return removed;
    }

    public void Add(string content)
    {
        var addedListElemObj = Instantiate(InputListElemPrefab, AddedContent);
        var addedListElem = addedListElemObj.GetComponent<InputListElem>();
        addedListElem.Set(this, content);

        AddedListElems.Add(addedListElem);
    }
}