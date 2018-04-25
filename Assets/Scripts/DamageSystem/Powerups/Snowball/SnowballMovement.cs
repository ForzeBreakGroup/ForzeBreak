using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballMovement : PowerUpMovement
{
    [SerializeField] private float initLaunch;
    [SerializeField] private float velocity;
    [SerializeField] private float maxScale;

    [Range(0, 1)]
    [SerializeField] private float growthFactor;
    [SerializeField] private float maxMass;

    private Rigidbody rb;
    private float currentScale;
    private float defaultMass;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * initLaunch, ForceMode.Impulse);
        defaultMass = rb.mass;
    }

    private void Update()
    {
        currentScale += Time.deltaTime * growthFactor;
        transform.localScale = new Vector3(currentScale, currentScale, currentScale);
        rb.mass = Time.deltaTime / growthFactor;
        rb.mass = Mathf.Clamp(rb.mass, defaultMass, maxMass);


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
