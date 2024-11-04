using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tutorial Text", menuName = "Tutorial Text")]
public class TutorialText : ScriptableObject
{
    [TextArea(3, 30)]
    public string finalText;
}
