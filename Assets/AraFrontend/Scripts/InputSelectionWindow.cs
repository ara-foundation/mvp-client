using Ara.RuntimeEditor;
using Lean.Gui;
using Rundo.RuntimeEditor.Behaviours.UI;
using Rundo.RuntimeEditor.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputSelectionWindow : MonoBehaviour
{
    [SerializeField] private LeanWindow Window;
    [SerializeField] private TextMeshProUGUI Title;
    [SerializeField] private Content _content;
    [SerializeField] private GameObject _prefabsWindowItemPrefab;
    [SerializeField] private ProjectItemsSearchFilterBehaviour _projectItemsSearchFilterBehaviour;
    [SerializeField] private PrefabScreenshoterBehaviour prefabScreenshoter;

    private AraFrontend.ActionType _actionType;
    private readonly List<SelectionWindowItem> _items = new List<SelectionWindowItem>();
    private readonly Dictionary<GameObject, Texture2D> _screenshotsCache = new Dictionary<GameObject, Texture2D>();
    private readonly Queue<SelectionWindowItem> _screenshotsQueue = new Queue<SelectionWindowItem>();

    private void ActionType(AraFrontend.ActionType actionType)
    {
        this._actionType = actionType;
        SetTitle();
    }

    private void SetTitle()
    {
        if (this._actionType == AraFrontend.ActionType.Input)
        {
            this.Title.text = "Input selection";
        } else if (this._actionType == AraFrontend.ActionType.Output)
        {
            this.Title.text = "Output selection";
        }  else if (this._actionType == AraFrontend.ActionType.Timer)
        {
            this.Title.text = "Timer for...";
        }
    }

    public void TurnOn(List<ProjectItemMetaData> selected, AraFrontend.ActionType actionType)
    {
        if (!Window.On)
        {
            Window.TurnOn();
        }
        ActionType(actionType);

        var data = new List<ProjectItemMetaData>();

        data.AddRange(selected);

        _projectItemsSearchFilterBehaviour.SetData(data, Redraw);

        Redraw(data);
    }

    public void TurnOff(bool ifOn)
    {
        if (ifOn && !Window.On)
        {
            return;
        }
        Window.TurnOff();
        _projectItemsSearchFilterBehaviour.ClearData();
    }

    private void OnItemSelect(ProjectItemMetaData metaData)
    {
        AraFrontend.Instance.OnItemSelect(metaData, _actionType);
    }

    private void Redraw(List<ProjectItemMetaData> data)
    {
        _content.Clear();
        _items.Clear();
        _screenshotsQueue.Clear();
            
        foreach (var it in data)
        {
            if (it.DiosType == null || it.DiosType.Count == 0)
            {
                var item = _content.Add<SelectionWindowItem>(_prefabsWindowItemPrefab);
                item.SetData(it, OnItemSelect);
                _items.Add(item);
                _screenshotsQueue.Enqueue(item);
            } else {
                var extracted = DIOSData.Extract(it);
                foreach (var ex in extracted)
                {
                    var item = _content.Add<SelectionWindowItem>(_prefabsWindowItemPrefab);
                    item.SetData(ex, OnItemSelect);
                    _items.Add(item);
                    _screenshotsQueue.Enqueue(item);
                }
            }
            
        }
        StartCoroutine(UpdateThumbnails());
    }

    private IEnumerator UpdateThumbnails()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            while (_screenshotsQueue.Count > 0)
            {
                var item = _screenshotsQueue.Dequeue();

                var go = item.Data.GameObject;
                if (go != null)
                {
                    if (_screenshotsCache.TryGetValue(go, out var screenshot))
                    {
                        item.UpdateScreenshot(screenshot);
                    }
                    else
                    {
                        var instance = Instantiate(go, null);
                        screenshot = prefabScreenshoter.Screenshot(instance);
                        Destroy(instance, 3f); // 3 seconds is enough to load the items and show the content.
                        _screenshotsCache[go] = screenshot;
                        item.UpdateScreenshot(screenshot);
                        break;
                    }
                }
            }
        }
    }
}
