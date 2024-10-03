using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserScenario_Problem : MonoBehaviour
{
    private UserScenarioProblem draft;
    [SerializeField] private TMP_InputField Describe;
    [SerializeField] private InputList Obstacles;

    public void Show(UserScenarioProblem problem)
    {
        draft = problem;
        Describe.text = problem.description;
        for (int i = 0; i < problem.obstacles.Length; i++)
        {
            Obstacles.Add(problem.obstacles[i]);
        }
    }

    public bool IsEmpty()
    {
        if (string.IsNullOrEmpty(Describe.text))
        {
            return true;
        }
        return Obstacles.IsEmpty();
    }

    public UserScenarioProblem Content()
    {
        draft.description = Describe.text;
        var obstacles = Obstacles.Elements();
        draft.obstacles = new string[obstacles.Length]; 
        for (var i = 0; i < obstacles.Length; i++)
        {
            draft.obstacles[i] = obstacles[i];
        }

        return draft;
    }
}
