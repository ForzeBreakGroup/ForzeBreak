using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Author: Robin Zhou
 * 
 * Description:
 * Control class of the cannon.
 */
public class CannonControl : PowerUpComponent
{
    /// <summary>
    /// Time duration for next fire
    /// </summary>
    public float FireCD = 0.5f;

    /// <summary>
    /// Bullet spawn point.
    /// </summary>
    private Transform bulletSpawn;


    private float nextFire = 0.0f;


    protected override void Awake()
    {
        base.Awake();
        bulletSpawn = transform.Find("BulletSpawn");
    }

    public override void AdjustModel()
    {
        base.AdjustModel();

        // Handling picking up same powerup
        CannonControl[] cannonControls = GetComponents<CannonControl>();
        if (cannonControls.Length > 1)
        {
            DestroyImmediate(this);
        }

    }

    protected override void OnPress() 
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + FireCD;
            base.OnPress();
        }
    }
}
