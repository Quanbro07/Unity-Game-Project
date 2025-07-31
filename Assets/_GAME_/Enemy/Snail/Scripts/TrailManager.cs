using System.Collections.Generic;
using UnityEngine;

public class TrailManager : MonoBehaviour
{
    public static TrailManager Instance;

    private List<TrailTracker> allTrails = new List<TrailTracker>();

    void Awake()
    {
        Instance = this;
    }

    public void RegisterTrail(TrailTracker tracker)
    {
        allTrails.Add(tracker);
    }

    public List<TrailTracker> GetAllTrails()
    {
        return allTrails;
    }
}
