using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballMovement : PowerUpMovement
{
    [SerializeField] private float initLaunch = 30.0f;
    [SerializeField] private float velocity = 500.0f;
    [SerializeField] private float maxScale = 2.0f;
    [SerializeField] private float growthFactor = 1.0f;

    private Rigidbody rb;
    private float currentScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * initLaunch, ForceMode.Impulse);
    }

    private void Update()
    {
        currentScale += Time.deltaTime * growthFactor;
        transform.localScale = new Vector3(currentScale, currentScale, currentScale);

        if (currentScale > maxScale)
        {
            DestroyPowerUpProjectile();
        }
        rb.AddForce(transform.forward * velocity * Time.deltaTime);
    }

    public float CurrentScaleRatio()
    {
        return currentScale / maxScale;
    }
}
