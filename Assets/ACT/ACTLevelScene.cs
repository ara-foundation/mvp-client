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

public class ACTLevelScene : EditorBaseBehaviour
{
    private List<ACTPart_interface> Parts = new ();

    public Camera Camera;
    public ActivityGroup ActivityGroup;

    [SerializeField] public LeanWindow LoadingSceneModal;
    [SerializeField] private LeanWindow PrimitivesWindow;
    [SerializeField] private LeanWindow LineWindow;
    [SerializeField] private GameObject BottomMenuPlane;
    [SerializeField] private GameObject LinePrefab;
    [SerializeField] private MouseInput BoxEnvironment;
    private static ACTLevelScene _instance;

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


    private void OnEnable()
    {
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
                AraRuntimeEditor_manager.Instance.LoadScene(manualSceneId);
                Debug.Log($"Loading the scene for {manualSceneId}");
            }
        }
    }

    private async Task<List<PlanWithProject>> FetchProject(string projectId, string levelId)
    {
        List<PlanWithProject> incorrectResult = new();

        string url = NetworkParams.AraActUrl + "/maydone/plans";

        string res;
        try
        {
            res = await WebClient.Get(url);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return incorrectResult;
        }

        List<PlanWithProject> result;
        try
        {
            result = JsonConvert.DeserializeObject<List<PlanWithProject>>(res);
        }
        catch (Exception e)
        {
            Debug.LogError(e + " for " + res);
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
            Debug.Log("TODO ActScene: Tech stack was set, let's now work on the budget. Add the budget UI controller");
            _part.Interactive(true);
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
