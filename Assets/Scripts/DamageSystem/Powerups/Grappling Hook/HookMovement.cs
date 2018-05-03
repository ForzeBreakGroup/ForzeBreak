using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Robin Zhou
 * 
 * Description:
 * Hook moving class.
 * 
 */
public class HookMovement : PowerUpMovement
{
    /// <summary>
    /// Bullet moving speed
    /// </summary>
    public float Velocity = 100f;
    /// <summary>
    /// Bullet existing time duration
    /// </summary>
    public float ExistingTime = 20f;

    public GameObject source;
    public GameObject target;

    private Transform trail;
    private LineRenderer line;
    private float spawnTime = 0f;
    private Vector3 offset = new Vector3(0, 1.308f, 1.808f);
    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Velocity * transform.forward;
        spawnTime = Time.time;
        trail = transform.Find("Trail");
        line = trail.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        line.SetPosition(0, trail.position);

        if(source!=null)
        {
            line.SetPosition(1, source.transform.position);
        }


        if (Time.time > spawnTime + ExistingTime)
        {
            DestroyPowerUpProjectile();
        }
    }
}
