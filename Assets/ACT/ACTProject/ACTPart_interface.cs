using Rundo.RuntimeEditor.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ACTPart_interface
{
    void Activate(DataGameObjectId objectId);
    string ObjectId();
}
