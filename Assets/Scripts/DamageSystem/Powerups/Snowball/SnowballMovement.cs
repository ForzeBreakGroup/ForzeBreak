using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballMovement : PowerUpMovement
{
    [SerializeField] private float velocity = 500.0f;
    [SerializeField] private float launchAngle = 45.0f;
    [SerializeField] private float maxScale;
    [SerializeField] private float growthFactor = 1.0f;

    private Rigidbody rb;
    private float currentScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log(transform.forward);

        rb.AddForce(transform.forward * velocity);
    }

    private void Update()
    {
        currentScale += Time.deltaTime * growthFactor;
        transform.localScale = new Vector3(currentScale, currentScale, currentScale);

        if (currentScale > maxScale)
        {
            DestroyPowerUpProjectile();
        }
    }

    public float CurrentScaleRatio()
    {
        return currentScale / maxScale;
    }
}
