using Lean.Gui;
using NBitcoin.Protocol;
using Rundo.RuntimeEditor.Behaviours.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

/// <summary>
/// Decentralized I/O System Data
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class DIOSObject : DIOSData
{
    public bool autoInitialize = false;
    [SerializeField] private Type DataType;
    [SerializeField] private GameObject AraFrontendDraggablePrefab;
    [SerializeField] private DIOSObjectDraggable AraFrontendDraggable;

    TextMeshProUGUI title;
    RectTransform myRectTransform;

    private void Start()
    {
        title = GetComponent<TextMeshProUGUI>();
        myRectTransform = GetComponent<RectTransform>();
        InitializeDraggable();
        InitializeData();
    }

    void InitializeDraggable()
    {
        if (AraFrontendDraggable != null)
        {
            AraFrontendDraggable.OnSelected += OnSelect;
            return;
        }

        AraFrontendDraggable = GetComponentInChildren<DIOSObjectDraggable>();
        if (!AraFrontendDraggable)
        {
            var obj = Instantiate(AraFrontendDraggablePrefab, transform);
            AraFrontendDraggable = obj.GetComponent<DIOSObjectDraggable>();
        }
        AraFrontendDraggable.OnSelected += OnSelect;
    }

    void InitializeData()
    {
        DataTypes = GameObjectDataType(gameObject, cache: false);
        DataType = DataTypes[0];

        var go = MainGameObject(gameObject, DataType, cache: false);
        if (go != null)
        {
            MainTarget = go;
            Payload = AutoPayload(go, DataType, cache: false);
        }
    }

    public void Highlight(bool enabled)
    {
        if (AraFrontendDraggable != null)
        {
            var rect = ScreenPosition(myRectTransform);
            //AraFrontendDraggable.Highlight(enabled);
        }
    } 

    public void Selectable(bool enabled)
    {
        if (enabled)
        {
            AraFrontendDraggable.Enable();
        } else
        {
            AraFrontendDraggable.Disable();
        }
    }

    void OnSelect()
    {
        Debug.Log($"The {title.text} was selected");
    }

    private void Update()
    {
        if (!autoInitialize)
        {
            autoInitialize = true;
            InitializeData();
            InitializeDraggable();
        }
    }

    private void OnEnable()
    {
        DIOSObjectRegistry.Add(this);
    }

    private void OnDisable()
    {
        DIOSObjectRegistry.Delete(this);
    }

    public Rect ScreenPosition()
    {
        return ScreenPosition(myRectTransform);
    }

    public Rect ScreenPosition(RectTransform rectTransform)
    {
        var mainCamera = AraFrontend.Instance.MainCamera;

        var corners = new Vector3[] {
                Vector3.zero,   // bottom left
                Vector3.zero,   // top left
                Vector3.zero,   // right top
                Vector3.zero,   // right bottom
        };

        
        rectTransform.GetWorldCorners(corners);

        // Any world corner within the range will be counted
        for (var i = 0; i < 4; i++)
        {
            corners[i] = mainCamera.WorldToScreenPoint(corners[i]);
        }

        return new Rect(corners[0].x,
                        corners[0].y,
                        corners[2].x - corners[0].x,
                        corners[2].y - corners[0].y);
    }
}
