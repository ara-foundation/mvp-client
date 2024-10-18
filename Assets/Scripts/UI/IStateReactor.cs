using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IStateReactor
{
    void Select(bool enabled);
    void Focus(bool enabled);

    void Highlight(bool enabled);

    void Clear();
}
