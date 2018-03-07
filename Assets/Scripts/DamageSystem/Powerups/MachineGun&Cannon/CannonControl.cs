﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonControl : PowerUpBase
{

    public float fireRate = 0.5f;
    public int ammo = 10;

    private Transform bulletSpawn;
    private float nextFire = 0.0f;

    private FMOD.Studio.EventInstance fireSound;

    private void Awake()
    {
        this.enabled = photonView.isMine;
        bulletSpawn = transform.Find("BulletSpawn");

        fireSound = FMODUnity.RuntimeManager.CreateInstance("event:/SFX_Diegetic/SFX_Cannon");
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

        // Move the weapon model to desired places
        //transform.localPosition = componentOffset;
        //transform.localRotation = Quaternion.identity;
    }

    protected override void OnHold() 
    {
        if(Time.time>nextFire)
        {
            nextFire = Time.time + fireRate;
            GameObject bullet = PhotonNetwork.Instantiate("Bullet", bulletSpawn.position, bulletSpawn.rotation,0);
            bullet.GetComponent<BulletMovement>().playerNum = playerNum;
            ammo--;


            FMODUnity.RuntimeManager.AttachInstanceToGameObject(fireSound, transform, GetComponent<Rigidbody>());
            fireSound.start();


            if (ammo<0)
            {
                UnloadPowerUp();
            }
        }
    }




}
