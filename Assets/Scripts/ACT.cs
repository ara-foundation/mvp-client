using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UIElements;

public class ACT : MonoBehaviour
{
    [Serializable]
    public enum WindowType
    {
        Projects,   // Shows the projects, for maintainers
        Tasks, // Shows the tasks, for contributors
        Today, // Shows the todo list marked for today, taken from end procrastination along with mushtra
        Calendar, // Shows the calendar along with todo, projects, meetings
        Habit, // Mushtra, daily routine
        GoalDream, // GoalDream is the backlog of all ideas that needs to be sorted out and expanded.
        Chats, // Telegram
        Meetings, // Video Calls
    }

    [SerializeField]
    public WindowType CurrentWindowType;

    [SerializeField] private ACTProjects ACTProjects;
    [SerializeField] private GameObject[] Backgrounds;

    private static ACT _instance;

    public static ACT Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ACT>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        ACTProjects.gameObject.SetActive(false);
        ShowDefault();
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        //await LoadIdeas();
    }

    public void SetWindowType(string windowTypeStr)
    {
        var valid = Enum.TryParse(windowTypeStr, out WindowType windowType);
        if (!valid)
        {
            Debug.LogError($"The '{windowTypeStr}' is not a valid ACT window type!");
            return;
        }

        CurrentWindowType = windowType;
    }

    private void ShowDefault()
    {
        if (Backgrounds != null && Backgrounds.Length > 0)
        {
            for (int i = 0; i < Backgrounds.Length; i++)
            {
                if (!Backgrounds[i].activeSelf)
                    Backgrounds[i].gameObject.SetActive(true);
            }
        }
    }

    private void HideDefault()
    {
        if (Backgrounds != null && Backgrounds.Length > 0)
        {
            for (int i = 0; i < Backgrounds.Length; i++)
            {
                if (Backgrounds[i].activeSelf)
                {
                    Backgrounds[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void OnWindow()
    {
        if (CurrentWindowType == WindowType.Projects)
        {
            HideDefault();
            ACTProjects.gameObject.SetActive(true);
        } else
        {
            ShowDefault();
        }
    }

    public void OffWindow()
    {
        if (CurrentWindowType == WindowType.Projects)
        {
            ShowDefault();
            ACTProjects.gameObject.SetActive(false);
        } else
        {
            HideDefault();
        }
    }

   
}
