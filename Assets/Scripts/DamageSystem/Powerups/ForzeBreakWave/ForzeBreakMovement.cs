using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForzeBreakMovement : PowerUpMovement
{
    public Vector3 StartScale = Vector3.zero;
    public Vector3 EndScale = Vector3.zero;
    public Vector3 StartPosition = Vector3.zero;
    public Vector3 EndPosition = Vector3.zero;
    /// <summary>
    /// Bullet existing time duration
    /// </summary>
    public float ExistingTime = 10f;

    private float spawnTime = 0f;
    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        spawnTime += Time.deltaTime;

        transform.localScale = Vector3.Lerp(StartScale, EndScale, spawnTime/ExistingTime);
        transform.position = Vector3.Lerp(StartPosition, EndPosition, spawnTime / ExistingTime);
        if (spawnTime>ExistingTime)
        {
            DestroyPowerUpProjectile();
        }
    }

}
