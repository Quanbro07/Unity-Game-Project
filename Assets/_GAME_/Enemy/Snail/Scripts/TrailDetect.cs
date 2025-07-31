using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailDetector : MonoBehaviour
{
    public float detectionRadius = 0.5f;
    private Health playerHealth;

    public float poisonTickCooldown = 1.0f;
    private bool isPoisonTicking = false;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
    }

    void Update()
    {
        if (isPoisonTicking) return;

        foreach (var tracker in TrailManager.Instance.GetAllTrails())
        {
            List<Vector3> points = tracker.GetTrailPoints();
            for (int i = 1; i < points.Count; i++)
            {
                Vector3 a = points[i - 1];
                Vector3 b = points[i];
                float distance = DistanceToSegment(transform.position, a, b);

                if (distance <= detectionRadius)
                {
                    StartCoroutine(ApplyPoisonTicks(3)); // 3 lần mỗi lần cooldown
                    return;
                }
            }
        }
    }

    private IEnumerator ApplyPoisonTicks(int tickCount)
    {
        isPoisonTicking = true;

        for (int i = 0; i < tickCount; i++)
        {
            playerHealth.TakePoisionDamage(2);
            yield return new WaitForSeconds(poisonTickCooldown);
        }

        isPoisonTicking = false;
    }

    // Tính khoảng cách từ điểm đến đoạn thẳng AB
    float DistanceToSegment(Vector3 point, Vector3 a, Vector3 b)
    {
        Vector3 ab = b - a;
        Vector3 ap = point - a;
        float t = Vector3.Dot(ap, ab) / ab.sqrMagnitude;
        t = Mathf.Clamp01(t);
        Vector3 projection = a + t * ab;
        return Vector3.Distance(point, projection);
    }
}
