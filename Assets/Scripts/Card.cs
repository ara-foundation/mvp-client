using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


[ExecuteInEditMode]
public class Card : MonoBehaviour
{
    private LeanDrag Drag;
    [SerializeField] private CardBody Body;
    public bool onlyHeader;
    private bool _onlyHeader;
    private bool _updateBodyHeight = false;
    [SerializeField] private float bodyHeight = 0.0f;

    [Description("Lungta Model, the previous card")]
    [SerializeField] public Card PrevCard;

    private RectTransform _bodyTransform;
    private RectTransform _transform;
    private int _index;

    private void OnEnable()
    {
        var found = gameObject.TryGetComponent<LeanDrag>(out Drag);
        if (!found) {
            throw new System.Exception("Card has no LeanDrag component");
        }
        _bodyTransform = Body.gameObject.GetComponent<RectTransform>();
        _transform = gameObject.GetComponent<RectTransform>();

        _index = 0;
    }

    public void CalculateView(int newIndex = 0)
    {
        _index = newIndex;
        if (_index > 0)
        {
            if (Drag != null) 
                Drag.interactable = false;
        }
        applyBodyHeight();
        Body.SetImageColor(CardColor.level[_index]);

        if (PrevCard != null )
        {
            PrevCard.onlyHeader = true;
            PrevCard.CalculateView(_index+1);
        }

    }

    // Update is called once per frame
    void Update()
    {
        // Call it before only header update, as this variable is defined after end of header update.
        if (_updateBodyHeight)
        {
            applyBodyHeight();
            _updateBodyHeight = false;
        }

        if (onlyHeader != _onlyHeader)
        {
            _onlyHeader = onlyHeader;
            applyOnlyHeader();
            _updateBodyHeight = true;
        }

        CalculateView(_index);
    }

    void applyBodyHeight()
    {
        if (_bodyTransform == null)
        {
            _bodyTransform = Body.gameObject.GetComponent<RectTransform>();
            _transform = gameObject.GetComponent<RectTransform>();
        }
        bodyHeight = _bodyTransform.sizeDelta.y;
        
        _transform.sizeDelta = new Vector2(_transform.sizeDelta.x, bodyHeight);

    }

    void applyOnlyHeader()
    {
        if (_bodyTransform == null)
        {
            _bodyTransform = Body.gameObject.GetComponent<RectTransform>();
            _transform = gameObject.GetComponent<RectTransform>();
        }
        Body.ShowContent(_onlyHeader);
    }
}
