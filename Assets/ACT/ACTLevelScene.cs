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
public class Level
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

/// <summary>
/// The ACT Scene where a person draws a diagram and plans the project.
/// </summary>
public class ACTLevelScene : EditorBaseBehaviour
{
    private List<ACTPart_interface> Parts = new ();

    public Camera Camera;
    public ActivityGroup ActivityGroup;

    public GameObject PartControllerPrefab;
    [SerializeField] public LeanWindow LoadingSceneModal;
    [SerializeField] private LeanWindow PrimitivesWindow;
    [SerializeField] private LeanWindow LineWindow;
    [SerializeField] private GameObject BottomMenuPlane;
    [SerializeField] private GameObject LinePrefab;
    [SerializeField] private MouseInput BoxEnvironment;
    private static ACTLevelScene _instance;

    private string _currentId = "";
    private Level _currentLevel = null;

    #region PartParameterEditing
    public enum TutorialStep
    {
        ProjectName = 0,
        TechStackStart = 1,
        TechStackEnd = 4,
        BudgetStart = 5,
        BudgetEnd = 6,
        Maintainer = 7,
        Tasks = 8,
    }
    public List<Event> PrimitiveTutorialTexts;
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

    public bool manualTest = false;
    [Tooltip("Loads a new empty scene, otherwise loads the predefined scene for a part")]
    public bool manualNewScene = false;
    private string manualProjectId = "67056baf24372ef24a58420c";
    public string manualSceneId = "67056baf24372ef24a58420c";

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
        //Debug.Log("C:\\Users\\milay\\AppData\\LocalLow\\DefaultCompany\\mvp-client to check the list of scene ids");
        if (ACTSession.Instance)
        {
            _currentId = ACTSession.Instance.Project._id;
            _currentLevel = await FetchScene(_currentId);
            if (!_currentLevel.Exist())
            {
                AraRuntimeEditor_manager.Instance.CreateNewScene();
            } else
            {
                AraRuntimeEditor_manager.Instance.LoadScene(_currentLevel.sceneId, _currentLevel.dataScene);
            }
        } else
        {
            Debug.LogWarning("No ACTSession was given which comes from Ara scene. Enable the manualTest flag on this level scene on inspector to use a standalone version");

            if (manualTest)
            {
                Debug.Log("Automatically loading manual project");
                Debug.Log($"Manual project id: {manualProjectId}");
                if (manualNewScene)
                {
                    Debug.Log("Loading a new empty scene");
                    AraRuntimeEditor_manager.Instance.CreateNewScene();
                } else
                {
                    //AraRuntimeEditor_manager.Instance.LoadScene(manualSceneId);
                    Debug.Log($"Loading the scene for {manualSceneId}");
                }
            }
        }
    }


    private async Task<Level> FetchScene(string id)
    {
        Level incorrectResult = new() { 
            sceneId = "",
        };

        string url = NetworkParams.AraActUrl + "/act/scenes/" + id;

        string res;
        try
        {
            res = await WebClient.Get(url);
        }
        catch (Exception ex)
        {
            return incorrectResult;
        }

        if (string.IsNullOrEmpty(res))
        {
            return incorrectResult;
        }

        Level result;
        try
        {
            result = JsonConvert.DeserializeObject<Level>(res);
        }
        catch (Exception e)
        {
            return incorrectResult;
        }
        return result;
    }

    private void Awake()
    {
        if (PrimitivesWindow != null)
        {
            PrimitivesWindow.gameObject.SetActive(false);
        }
        if (LineWindow != null)
        {
            LineWindow.gameObject.SetActive(false);
        }
    }

    void Update ()
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
        LineWindow.gameObject.SetActive(selected);
        LineWindow.Set(selected);
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
        } else
        {
            if (!Parts.Contains(part))
            {
                Parts.Add(part);

                if (_nextPartForEditing)
                {
                    _nextPartForEditing = false;
                    StartCoroutine(StartEditingPart(part));
                }
            }
        }
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
        } else
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

    IEnumerator StartEditingPart(ACTPart_interface part)
    {
        yield return 0; // wait for the next frame

        _editingPart = part;
        var _part = _editingPart as ACTPart;

        // Focus the camera on the part.
        // The drawback is that transform handler is also activated.
        _part.SetEditMode(true);
        _part.EditProjectName(OnProjectNameEdited);

        // Start tutorial
        _editingStep = Sensever_dialogue.None;
        Sensever_window.Instance.ShowSensever(PrimitiveTutorialTexts, OnDialogueEnd, OnDialogueStart, HideSenseverInsteadContinue);
    }

    private void OnProjectNameEdited(string name, bool submitted)
    {
        var _part = _editingPart as ACTPart;
        
        if (submitted)
        {
            Debug.Log("TODO ActScene: Project name was set, let's now work on the tech stack by continuing sensever. Also we must call UI change");
            Sensever_window.Instance.ContinueSensever((int)TutorialStep.TechStackStart);
            _part.EditTechStack(OnTechStackEdited);
        } else
        {
            //Notification.Instance.Show("To continue set the project name first.");
            Debug.LogWarning("To continue set the project name first.");
            _part.EditProjectName(OnProjectNameEdited);
            return;
        }
    }

    private void OnTechStackEdited(string name, bool submitted)
    {
        var _part = _editingPart as ACTPart;

        if (submitted)
        {
            OnEditingEnd();
            Debug.Log("TODO ActScene: Tech stack was set, let's now work on the budget. Add the budget UI controller");
            //Sensever_window.Instance.ContinueSensever((int)TutorialStep.TechStackStart);
            //_part.EditTechStack(OnTechStackEdited);
        }
        else
        {
            //Notification.Instance.Show("To continue set the project name first.");
            Debug.LogWarning("To continue set the tech stack first.");
            _part.EditTechStack(OnTechStackEdited);
            return;
        }
    }

    private void OnEditingEnd()
    {
        var _part = _editingPart as ACTPart;
        _part.Interactive(true);
        ACTLevels.Instance.OnSceneUpdate(OnSaveScene);
    }

    public void OnLineEditingEnd(DataGameObjectId dataGameObjectId, List<string> connection)
    {
        if (_currentLevel == null)
        {
            return;
        }
        if (_currentLevel.lines == null)
        {
            _currentLevel.lines = new();
        }
        _currentLevel.lines.Add(dataGameObjectId.ToStringRawValue(), connection);

        ACTLevels.Instance.OnSceneUpdate(OnSaveScene);
    }

    public List<string> LineConnections(DataGameObjectId dataGameObjectId)
    {
        if (_currentLevel == null)
        {
            return null;
        }

        if (_currentLevel.lines == null)
        {
            return null;
        }

        if (!_currentLevel.lines.ContainsKey(dataGameObjectId.ToStringRawValue())) {
            return null;
        }

        return _currentLevel.lines[dataGameObjectId.ToStringRawValue()];
    }

    private async void OnSaveScene()
    {
        _currentLevel.dataScene = RuntimeEditorController.GetSerializedDataScene();
        _currentLevel.sceneId = AraRuntimeEditor_manager.Instance.GetSceneId(RuntimeEditorController.TabGuid).ToStringRawValue();

        await SaveScene(_currentId, _currentLevel);
    }

    private async Task SaveScene(string _id, Level _level)
    {
        var body = JsonConvert.SerializeObject(_level);
        string url = NetworkParams.AraActUrl + "/act/scenes/" + _id;

        Tuple<long, string> res;
        try
        {
            res = await WebClient.Post(url, body);
        }
        catch (Exception ex)
        {
            Notification.Instance.Show($"Error: web client exception {ex.Message}");
            Debug.LogError(ex);
            return;
        }
        if (res.Item1 != 200)
        {
            Notification.Instance.Show($"Error: {res.Item2}");
            return;
        }

        Notification.Instance.Show($"Scene was saved successfully!");
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
        Debug.Log($"Tutorial ended for {showed} is it this one {_editingStep}");
    }

    private void OnDialogueStart(int started)
    {
        _editingStep = started;
        Debug.Log("Starting the dialogue text for " + (TutorialStep)started);
    }

    private bool HideSenseverInsteadContinue(int showedStep)
    {
        Debug.Log($"Hide Sensever instead continuing? {(TutorialStep)showedStep}");
        // Once we show the project name, let's hide the sensever tutorial until user completes the project name
        if (showedStep == (int)TutorialStep.ProjectName)
        {
            return true;
        } else 
        // Tech stack has multiple data, therefore show them all.
        if (showedStep >= (int)TutorialStep.TechStackStart && showedStep < (int)TutorialStep.TechStackEnd)
        {
            return false;
        } else 
        // Once we show the tech stack, let's hide the sensever tutorial until user completes the tech stack
        if (showedStep == (int)TutorialStep.TechStackEnd)
        {
            return true;
        } else if (showedStep >= (int)TutorialStep.BudgetStart && showedStep < (int)TutorialStep.BudgetEnd)
        {
            return false;
        } else if (showedStep == (int)TutorialStep.BudgetEnd)
        {
            return true;
        } else if (showedStep == (int)TutorialStep.Maintainer)
        {
            return false;
        } else 
        // The last step, therefore hide the tutorial
        if (showedStep == (int)TutorialStep.Tasks)
        {
            return true;
        } else if (showedStep == (int)Sensever_dialogue.None)
        {
            return false;
        }
        throw new System.Exception("Invalid step");
    }
}
