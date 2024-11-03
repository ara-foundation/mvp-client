using System.Collections.Generic;
using Rundo.Core.Data;
using Rundo.RuntimeEditor.Data;
using Rundo.RuntimeEditor.Tools;
using RuntimeHandle;
using UnityEngine;
using UnityEngine.UI;

namespace Rundo.RuntimeEditor.Behaviours.UI
{
    public class AraEditorUiMenuBehaviour : EditorBaseBehaviour
    {
        [SerializeField] private Button _transformPositionBtn;
        [SerializeField] private Button _transformRotationBtn;
        [SerializeField] private Button _transformScaleBtn;

        private void Start()
        {
            _transformPositionBtn.onClick.AddListener(() =>
            {
                DispatchUiEvent(new SelectionBehaviour.SetTransformHandleType{HandleType = HandleType.POSITION});
            });
            
            _transformRotationBtn.onClick.AddListener(() =>
            {
                DispatchUiEvent(new SelectionBehaviour.SetTransformHandleType{HandleType = HandleType.ROTATION});
            });

            _transformScaleBtn.onClick.AddListener(() =>
            {
                DispatchUiEvent(new SelectionBehaviour.SetTransformHandleType{HandleType = HandleType.SCALE});
            });
        }
    }
}


