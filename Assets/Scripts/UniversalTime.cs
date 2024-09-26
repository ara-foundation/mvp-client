using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UniversalTime
{
    static public long Now()
    {
        var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
        return (long)timeSpan.TotalSeconds;
    }
}
