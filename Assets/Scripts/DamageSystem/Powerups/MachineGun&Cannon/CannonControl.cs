using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonControl : PowerUpBase
{

    public float fireRate = 0.5f;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawn;
    private float nextFire = 0.0f;

    protected override void OnHold() 
    {
        if(Time.time>nextFire)
        {
            nextFire = Time.time + fireRate;
            Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
        }
    }


}
