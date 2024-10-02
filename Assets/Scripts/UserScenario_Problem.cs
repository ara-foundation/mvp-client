using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserScenario_Problem : MonoBehaviour
{
    [SerializeField] private TMP_InputField Describe;
    [SerializeField] private InputList Obstacles;

    public void Show(UserScenarioProblem problem)
    {
        Describe.text = problem.description;
        for (int i = 0; i < problem.obstacles.Length; i++)
        {
            Obstacles.Add(problem.obstacles[i]);
        }
    }
}
