using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleComponent : PowerUpComponent
{
    LineRenderer lineRender;

    [SerializeField]
    private int resolution = 10;

    [SerializeField]
    private float velocity = 100f;

    private float radianAngle;
    private float g;

    Transform launchPoint;

    protected override void Awake()
    {
        base.Awake();
        lineRender = GetComponent<LineRenderer>();
        g = Mathf.Abs(Physics.gravity.y);
        launchPoint = transform.Find("BlackHoleLaunchPoint");

        enabled = photonView.isMine;
        RenderArc();
    }

    protected override void OnPress()
    {
        if (spawnItem != null)
        {
            DecreaseCapacity();
            GameObject spawnedItem = PhotonNetwork.Instantiate(spawnItem.name, launchPoint.position, launchPoint.rotation, 0);
            ((PowerUpData)spawnedItem.GetComponent(typeof(PowerUpData))).SetOwnerId(this.ownerID);
        }
    }

    private void RenderArc()
    {
        lineRender.positionCount = resolution + 1;
        lineRender.SetPositions(CalculateProjectionArcArray());
    }

    private Vector3[] CalculateProjectionArcArray()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];

        radianAngle = Mathf.Deg2Rad * 45f;
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
