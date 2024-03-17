using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class StopWatchManager
{
    private static readonly List<Stopwatch> StopWatchList = new List<Stopwatch>();

    public static void AddEntity(Stopwatch e)
    {
        StopWatchList.Add(e);
    }

    public static void RemoveEntity(Stopwatch e)
    {
        StopWatchList.Remove(e);
    }

    public static void HangAll()
    {
        foreach (var e in StopWatchList)
        {
            e.Stop();
        }
    }

    public static void ResumeAll()
    {
        foreach (var e in StopWatchList)
        {
            e.Start();
        }
    }
}
