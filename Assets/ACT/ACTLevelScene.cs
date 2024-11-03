using Lean.Gui;
using Rundo.RuntimeEditor.Behaviours.UI;
using Rundo.RuntimeEditor.Behaviours;
using Rundo.RuntimeEditor.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rundo.RuntimeEditor.Commands;
using Rundo;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using Ara.RuntimeEditor;

[Serializable]
public class ACTScene
{
    public string sceneId;
#nullable enable
    public string? dataScene;
    public Dictionary<string, List<string>>? lines;
#nullable disable

    public bool Exist()
    {
        return (!string.IsNullOrEmpty(dataScene) && !string.IsNullOrEmpty(sceneId));
    }
}

[Serializable]
public class ACTPartModel
{
    public string _id;
    //
    // Meta params
    //
    public string objId;          // on scene
    public string developmentId; // project it belongs too
    public int level;        // level at which it resides
    public string[] childObjsId;
    public string parentObjId;

    // 
    // The part params
    //
    public string projectName;
    public string techStack;
    public int deadline;
    public decimal budget;
    public string maintainer;

#nullable enable
    public decimal? usedBudget;
#nullable disable

}

/// <summary>
/// The ACT Scene where a person draws a diagram and plans the project.
/// </summary>
public class ACTLevelScene : EditorBaseBehaviour
{
    private readonly List<ACTPart_interface> Parts = new();
    private ACTPartModel[] PartData = new ACTPartModel[] { };

    public Camera Camera;
    public ActivityGroup ActivityGroup;

    public GameObject PartControllerPrefab;
    [SerializeField] public LeanWindow LoadingSceneModal;
    [SerializeField] private LeanWindow PrimitivesWindow;
    [SerializeField] private GameObject BottomMenuPlane;
    [SerializeField] private GameObject LinePrefab;
    [SerializeField] private MouseInput BoxEnvironment;
    private static ACTLevelScene _instance;

    private string _currentDevelopmentId = "";
    private ACTScene _currentLevelScene = null;
    private int _currentLevel = 0;

    #region PartParameterEditing
    public enum TutorialStep
    {
        ProjectName = 0,
        TechStackStart = 1,
        TechStackEnd = 4,
        BudgetStart = 5,
        BudgetEnd = 6,
        Maintainer = 7,
        Save = 8,
        Congrats = 9,
        Tasks = 10,
    }
    public enum LineTutorialStep
    {
        Draw = 0,
    }
    public List<Event> PrimitiveTutorialTexts;
    public List<Event> LineTutorialTexts;
    /// <summary>
    /// When a AddPart method is called, and this flag is true. Then 
    /// start the editing mode.
    /// </summary>
    private bool _nextPartForEditing = false;
    private ACTPart_interface _editingPart = null;
    private int _editingStep = Sensever_dialogue.None;
    #endregion

    #region LineEditingVariables
    private bool _lineMode = false;
    private DataGameObject _editingLineData = null;
    private ACTPart_line _editingLinePart = null;
    #endregion

    public static ACTLevelScene Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ACTLevelScene>();
            }
            return _instance;
        }
    }

    private async void OnEnable()
    {
        if (ACTSession.Instance)
        {
            _currentDevelopmentId = ACTSession.Instance.DevelopmentId;
            _currentLevelScene = await ACTSession.Instance.CurrentScene();
            _currentLevel = ACTSession.Instance.Level;
            if (!_currentLevelScene.Exist())
            {
                AraRuntimeEditor_manager.Instance.CreateNewScene();
            }
            else
            {
                AraRuntimeEditor_manager.Instance.LoadScene(_currentLevelScene.sceneId, _currentLevelScene.dataScene);
            }
        }
        else
        {
            Debug.LogError("No ACTSession was given which comes from Ara scene. Come from the Ara scene");
        }
    }

    public async void OnSceneLoaded()
    {
        PartData = await ACTSession.Instance.CurrentParts();
    }
    private void Awake()
    {
        if (PrimitivesWindow != null)
        {
            PrimitivesWindow.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (_lineMode)
        {
            if (Input.GetMouseButtonDown(1))
            {
                UnsetLineMode(cancel: true);
            }
        }
    }

    private void UnsetLineMode(bool cancel)
    {
        _lineMode = false;
        if (cancel)
        {
            DestroyDataGameObjectCommand.Process(DataScene, _editingLineData);
        }
        SetPartsLineMode(false);
    }

    // Update is called once per frame
    public void OnPrimitivesWindowSelect(bool selected)
    {
        PrimitivesWindow.gameObject.SetActive(selected);
        PrimitivesWindow.Set(selected);
    }

    public void OnLineWindowSelect(bool selected)
    {
        // Start tutorial
        if (selected)
        {
            _editingStep = Sensever_dialogue.None;
            Sensever_window.Instance.ShowSensever(LineTutorialTexts, OnLineDialogueEnd, OnLineDialogueStart, HideSenseverInsteadContinueForLine);
        }
        SetLineMode();
    }

    IEnumerator StartEditingPart(ACTPart_interface part)
    {
        yield return 0; // wait for the next frame

        _editingPart = part;
        var _part = _editingPart as ACTPart;

        // Focus the camera on the part.
        // The drawback is that transform handler is also activated.
        _part.SetEditMode(true);
        _part.SetData(_currentDevelopmentId, _currentLevel, ACTSession.Instance.CurrentParentObjectId());
        _part.EditProjectName(OnProjectNameEdited);

        // Start tutorial
        _editingStep = Sensever_dialogue.None;
        Sensever_window.Instance.ShowSensever(PrimitiveTutorialTexts, OnDialogueEnd, OnDialogueStart, HideSenseverInsteadContinue);

    }

    public void AddPart(ACTPart_interface part)
    {
        var partType = part.GetType();
        if (partType == typeof(ACTPart_line))
        {
            if (_lineMode)
            {
                _editingLinePart = part as ACTPart_line;
            }
        }
        else
        {
            if (!Parts.Contains(part))
            {
                Parts.Add(part);

                if (_nextPartForEditing)
                {
                    _nextPartForEditing = false;
                    StartCoroutine(StartEditingPart(part));
                }
                else
                {
                    SetPartData(part);
                }
            }
        }
    }

    private void SetPartData(ACTPart_interface actPart)
    {
        if (PartData == null)
        {
            return;
        }

        foreach (var model in PartData)
        {
            if (model.objId.Equals(actPart.ObjectId()))
            {
                actPart.SetData(model);
                return;
            }
        }

        actPart.SetData(_currentDevelopmentId, _currentLevel, ACTSession.Instance.CurrentParentObjectId());
    }

    public ACTPart_interface GetPart(string objectId)
    {
        foreach (var part in Parts)
        {
            if (part.ObjectId().Equals(objectId))
            {
                return part;
            }
        }

        return null;
    }

    public void RemovePart(ACTPart_interface part)
    {
        var partType = part.GetType();
        if (partType == typeof(ACTPart_line))
        {
            if (_lineMode)
            {
                _editingLinePart = null;
            }
        }
        else
        {
            if (Parts.Contains(part))
            {
                Parts.Remove(part);
            }
        }
    }

    public void SetPartsInteractiveMode(bool on)
    {
        foreach (ACTPart part in Parts)
        {
            part.Interactive(on);
        }
        BoxEnvironment.enabled = on;
    }

    private void OnProjectNameEdited(string name, bool submitted)
    {
        var _part = _editingPart as ACTPart;

        if (submitted)
        {
            Sensever_window.Instance.ContinueSensever((int)TutorialStep.TechStackStart);
            _part.EditTechStack(OnTechStackEdited);
        }
        else
        {
            Notification.Instance.Show("To continue set the project name first.");
            _part.EditProjectName(OnProjectNameEdited);
            return;
        }
    }

    private void OnTechStackEdited(string name, bool submitted)
    {
        var _part = _editingPart as ACTPart;

        if (submitted)
        {
            Sensever_window.Instance.ContinueSensever((int)TutorialStep.BudgetStart);
            _part.EditBudget(OnBudgetEdited);
        }
        else
        {
            Notification.Instance.Show("To continue set the tech stack first.");
            _part.EditTechStack(OnTechStackEdited);
            return;
        }
    }

    private void OnBudgetEdited(decimal budget, bool submitted)
    {
        var _part = _editingPart as ACTPart;

        if (submitted)
        {
            Sensever_window.Instance.ContinueSensever((int)TutorialStep.Maintainer);
            _part.EditMaintainer(OnMaintainerEdited);
        }
        else
        {
            Notification.Instance.Show("To continue set the budget first.");
            _part.EditBudget(OnBudgetEdited);
            return;
        }
    }

    private void OnMaintainerEdited(string name, bool submitted)
    {
        var _part = _editingPart as ACTPart;

        if (submitted)
        {
            ACTLevels.Instance.OnSceneUpdate(OnSaveScene);
            Sensever_window.Instance.ContinueSensever((int)TutorialStep.Save);
        }
        else
        {
            Notification.Instance.Show("To continue set the budget first.");
            _part.EditBudget(OnBudgetEdited);
            return;
        }
    }

    private async void OnEditingEnd()
    {
        var _part = _editingPart as ACTPart;

        _part.SetEditMode(false);
        _part.Interactive(true);
        var saved = await _part.SaveModel();
        if (!saved.Item1)
        {
            Notification.Instance.Show("Failed to save the part in the server. Please try again later :(");
            return;
        }

        Sensever_window.Instance.ContinueSensever((int)TutorialStep.Congrats);
        var partModel = ACTSession.Instance.CurrentPart();
        if (partModel != null)
        {
            Debug.Log("The part has a parent part. Let's add the new object: ");

            if (partModel.childObjsId == null)
            {
                partModel.childObjsId = new string[] { _part.ObjectId()};
            }
            else
            {
                var updatedChildObjsId = new string[partModel.childObjsId.Length + 1];
                for (var i = 0; i < partModel.childObjsId.Length; i++)
                {
                    updatedChildObjsId[i] = partModel.childObjsId[i];
                }
                updatedChildObjsId[updatedChildObjsId.Length - 1] = _part.ObjectId();

                partModel.childObjsId = updatedChildObjsId;
            }

            ACTSession.Instance.CurrentPart(partModel);
        }
    }
    public void OnLineEditingEnd(DataGameObjectId dataGameObjectId, List<string> connection)
    {
        if (_currentLevelScene == null)
        {
            return;
        }
        if (_currentLevelScene.lines == null)
        {
            _currentLevelScene.lines = new();
        }
        _currentLevelScene.lines.Add(dataGameObjectId.ToStringRawValue(), connection);

        ACTLevels.Instance.OnSceneUpdate(OnSaveScene);
    }

    public List<string> LineConnections(DataGameObjectId dataGameObjectId)
    {
        if (_currentLevelScene == null)
        {
            return null;
        }

        if (_currentLevelScene.lines == null)
        {
            return null;
        }

        if (!_currentLevelScene.lines.ContainsKey(dataGameObjectId.ToStringRawValue()))
        {
            return null;
        }

        return _currentLevelScene.lines[dataGameObjectId.ToStringRawValue()];
    }

    private async void OnSaveScene()
    {
        _currentLevelScene.dataScene = RuntimeEditorController.GetSerializedDataScene();
        _currentLevelScene.sceneId = AraRuntimeEditor_manager.Instance.GetSceneId(RuntimeEditorController.TabGuid).ToStringRawValue();

        var sceneSaved = await SaveScene(_currentDevelopmentId, _currentLevelScene);
        if (sceneSaved && _editingPart != null)
        {
            OnEditingEnd();
        }
    }

    private async Task<bool> SaveScene(string _id, ACTScene _level)
    {
        var body = JsonConvert.SerializeObject(_level);
        string url = NetworkParams.AraActUrl + "/act/scenes/" + _id;
        if (_currentLevel > 1)
        {
            url += $"/{_currentLevel}/{ACTSession.Instance.CurrentParentObjectId()}";
            Debug.Log($"Save the nested scene at url: {url}");
        }

        Tuple<long, string> res;
        try
        {
            res = await WebClient.Post(url, body);
        }
        catch (Exception ex)
        {
            Notification.Instance.Show($"Error: web client exception {ex.Message}");
            Debug.LogError(ex);
            return false;
        }
        if (res.Item1 != 200)
        {
            Notification.Instance.Show($"Error: {res.Item2}");
            return false;
        }

        Notification.Instance.Show($"Scene was saved successfully!");
        return true;
    }

    /// <summary>
    /// This function makes sure that the next element added into the scene requires a meta data
    /// editing.
    /// 
    /// For meta data editing we use Sensever, Set the object as the target.
    /// </summary>
    public void FlagNextPartForEditing()
    {
        _nextPartForEditing = true;
    }

    private void SetPartsLineMode(bool on)
    {
        foreach (ACTPart part in Parts)
        {
            part.SetLineMode(on);
        }
        BoxEnvironment.enabled = on;
    }

    /// <summary>
    /// Changes the scene to the line placing mode.
    /// This method is called from Sensever Dialogue
    /// </summary>
    public void SetLineMode()
    {
        _lineMode = true;
        SetPartsLineMode(true);
        InstantiateLine();
    }

    private void InstantiateLine()
    {
        if (LinePrefab.TryGetComponent<PrefabIdBehaviour>(out var prefabIdBehaviour))
        {
            _editingLineData = DataScene.InstantiateDataGameObjectFromPrefab(prefabIdBehaviour);

            _editingLineData.GetComponent<DataGameObjectBehaviour>().DataComponentPrefab.OverridePrefabComponent = true;
            _editingLineData.GetComponent<DataTransformBehaviour>().DataComponentPrefab.OverridePrefabComponent = true;

            // Sends to the Rundo to add the line.
            // The instantiniated game object will call the ACTPart_interface.Activate method.
            // The ACTPart_interface.Activate calls ACTLevelScene.AddPart back.
            CreateDataGameObjectCommand.Process(DataScene, _editingLineData, DataScene);
        }
    }

    public void OnLinePointSelect(ACTPart part)
    {
        if (_editingLinePart != null)
        {
            if (_editingLinePart.OnSetPoint(part))
            {
                UnsetLineMode(cancel: false);
            }
        }
    }

    ///////////////////////////////////////////////////////////////////
    ///
    ///
    ///
    ///////////////////////////////////////////////////////////////////

    private void OnDialogueEnd(int showed)
    {
        if (showed == (int)TutorialStep.Congrats)
        {
            Notification.Instance.Show("Todo: Your reward is 1$ Dollar worth ARA, you can claim them in the Sangha Page");
            Debug.Log("Todo: After implementing tasks and sangha, reward the user with the items for this create reward popup");
        }
    }

    private void OnDialogueStart(int started)
    {
        _editingStep = started;
    }

    private void OnLineDialogueEnd(int _)
    {
    }

    private void OnLineDialogueStart(int _)
    {
    }

    private bool HideSenseverInsteadContinue(int showedStep)
    {
        // Once we show the project name, let's hide the sensever tutorial until user completes the project name
        if (showedStep == (int)TutorialStep.ProjectName)
        {
            return true;
        }
        else
        // Tech stack has multiple data, therefore show them all.
        if (showedStep >= (int)TutorialStep.TechStackStart && showedStep < (int)TutorialStep.TechStackEnd)
        {
            return false;
        }
        else
        // Once we show the tech stack, let's hide the sensever tutorial until user completes the tech stack
        if (showedStep == (int)TutorialStep.TechStackEnd)
        {
            return true;
        }
        else if (showedStep >= (int)TutorialStep.BudgetStart && showedStep < (int)TutorialStep.BudgetEnd)
        {
            return false;
        }
        else if (showedStep == (int)TutorialStep.BudgetEnd)
        {
            return true;
        }
        else if (showedStep == (int)TutorialStep.Maintainer)
        {
            return true;
        }
        else
        // The last step, therefore hide the tutorial
        if (showedStep == (int)TutorialStep.Tasks)
        {
            return true;
        }
        else
        // The last step, therefore hide the tutorial
        if (showedStep == (int)TutorialStep.Congrats)
        {
            return true;
        }
        else
        // The last step, therefore hide the tutorial
        if (showedStep == (int)TutorialStep.Save)
        {
            return true;
        }
        else if (showedStep == (int)Sensever_dialogue.None)
        {
            return false;
        }
        throw new System.Exception("Invalid step");
    }

    private bool HideSenseverInsteadContinueForLine(int _)
    {
        return true;
    }
}