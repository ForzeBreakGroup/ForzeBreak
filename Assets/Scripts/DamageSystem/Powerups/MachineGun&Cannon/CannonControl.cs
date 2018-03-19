using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Author: Robin Zhou
 * 
 * Description:
 * Control class of the cannon.
 */
public class CannonControl : PowerUpBase
{
    /// <summary>
    /// Time duration for next fire
    /// </summary>
    public float FireCD = 0.5f;
    /// <summary>
    /// Total Ammo, run out then destroy cannon
    /// </summary>
    public int Ammo = 10;
    /// <summary>
    /// Bullet spawn point.
    /// </summary>
    private Transform bulletSpawn;


    private float nextFire = 0.0f;


    private void Awake()
    {
        this.enabled = photonView.isMine;
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

    protected override void OnHold() 
    {
        if(Time.time>nextFire)
        {
            nextFire = Time.time + FireCD;
            GameObject bullet = PhotonNetwork.Instantiate("Bullet", bulletSpawn.position, bulletSpawn.rotation,0);

            Ammo--;

            if (Ammo<0)
            {
                UnloadPowerUp();
            }
        }
    }




}
