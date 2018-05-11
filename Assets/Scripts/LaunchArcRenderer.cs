using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaunchArcRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private float radianAngle;
    private float g;

    [SerializeField]
    private int resolution = 100;

    [Range(0, 90)]
    [SerializeField]
    private float angle = 45f;

    [SerializeField]
    private float velocity = 20f;

    private void Awake()
    {
        if (GetComponent<PhotonView>().isMine)
        {
            lineRenderer = GetComponent<LineRenderer>();
            g = Mathf.Abs(Physics.gravity.y);
            RenderArc();
        }
    }

    private void RenderArc()
    {
        lineRenderer.positionCount = resolution + 1;
        lineRenderer.SetPositions(CalculateProjectionArcArray());
    }

    private Vector3[] CalculateProjectionArcArray()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];

        radianAngle = Mathf.Deg2Rad * angle;
        float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / g;

        for (int t = 0; t <= resolution; ++t)
        {
            arcArray[t] = CalculateArcPoint((float)t / (float)resolution, maxDistance);
        }

        return arcArray;
    }

    private Vector3 CalculateArcPoint(float t, float maxDistance)
    {
        float x = t * maxDistance;
        float y = x * Mathf.Tan(radianAngle) - ((g * x * x) / (2 * velocity * velocity * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));
        return new Vector3(0, y, x);
    }
}
