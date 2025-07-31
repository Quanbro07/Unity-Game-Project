using System.Collections.Generic;
using UnityEngine;

public class TrailTracker : MonoBehaviour
{
    public float interval = 0.2f; // Mỗi bao lâu lưu 1 điểm
    public float maxTrailTime = 5f; // Giữ vết trong 5s
    private float timer = 0f;

    private List<Vector3> trailPoints = new List<Vector3>();
    private List<float> timestamps = new List<float>();

    private void Start()
    {
        TrailManager.Instance.RegisterTrail(this);
    }
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            trailPoints.Add(transform.position);
            timestamps.Add(Time.time);
            timer = 0f;
        }

        // Xoá điểm cũ quá maxTrailTime
        while (timestamps.Count > 0 && Time.time - timestamps[0] > maxTrailTime)
        {
            trailPoints.RemoveAt(0);
            timestamps.RemoveAt(0);
        }
    }

    public List<Vector3> GetTrailPoints()
    {
        return trailPoints;
    }
}
