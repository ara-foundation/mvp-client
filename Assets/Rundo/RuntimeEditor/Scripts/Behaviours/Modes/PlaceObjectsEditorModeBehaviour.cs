using Rundo.Core.Commands;
using Rundo.RuntimeEditor.Commands;
using Rundo.RuntimeEditor.Data;
using UnityEngine;

namespace Rundo.RuntimeEditor.Behaviours
{
    /// <summary>
    /// Used for placing prefabs over the zero Y-plane
    /// </summary>
    public class PlaceObjectsEditorModeBehaviour : EditorModeBaseBehaviour
    {
        private DataGameObject _dataGameObject;
        private GameObject _gameObject;
        private Plane _plane;

        private void Update()
        {
            // cancel mode
            if (Input.GetMouseButtonDown(1))
            {
                RuntimeEditorController.SetMode<SelectObjectsEditorModeBehaviour>();
                return;
            }

            // mouse raycast
            if (_dataGameObject != null)
            {
                var ray = RuntimeEditorController.ActiveCamera.ScreenPointToRay(Input.mousePosition);
                if (_plane.Raycast(ray, out var distance))
                {
                    var hitPoint = ray.GetPoint(distance);
                    var worldPos = new Vector3(hitPoint.x, hitPoint.y, hitPoint.z);

                    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                    {
                        worldPos.x = Mathf.RoundToInt(worldPos.x);
                        worldPos.z = Mathf.RoundToInt(worldPos.z);
                    }

                    // sync datamodel with cursor position
                    var dataTransformBehaviour = _dataGameObject.GetComponent<DataTransformBehaviour>();
                    var command = new SetValueToMemberCommand(dataTransformBehaviour.Data, nameof(DataTransformBehaviour.Position), worldPos);
                    command.SetIgnoreUndoRedo();
                    command.AddDispatcherData(dataTransformBehaviour);
                    DataScene.CommandProcessor.Process(command);
                }

                // place
                if (Input.GetMouseButtonDown(0))
                {
                    ACTLevelScene.Instance.FlagNextPartForEditing();
                    CreateDataGameObjectCommand.Process(DataScene, RundoEngine.DataSerializer.Clone(_dataGameObject), DataScene);
                    RuntimeEditorController.SetMode<SelectObjectsEditorModeBehaviour>();
                }
            }
        }

        public async void SetData(DataGameObject dataGameObject)
        {
            RuntimeEditorController.SelectionBehaviour.ClearSelection();
            
            _dataGameObject = dataGameObject;
            
            _gameObject = await DataScene.InstantiateGameObject(BaseDataProvider, _dataGameObject, null, true);
            _gameObject.transform.SetParent(transform, true);
            
            RuntimeEditorController.SelectionBehaviour.AddToSelectionWithoutTransformGizmo(_dataGameObject);
        }

        public override void Activate()
        {
            _plane = new Plane(Vector3.up, 0.01f);
            ACTLevelScene.Instance.SetPartsInteractiveMode(false);
            ACTLevelScene.Instance.OnPrimitivesWindowSelect(false);
        }

        /// <summary>
        /// This function is called automatically be Rundo, when the packages switches
        /// mode.
        /// </summary>
        public override void Deactivate()
        {
            RuntimeEditorController.SelectionBehaviour.ClearSelection();
            Destroy(_gameObject);
            ACTLevelScene.Instance.SetPartsInteractiveMode(true);
        }
    }
}


