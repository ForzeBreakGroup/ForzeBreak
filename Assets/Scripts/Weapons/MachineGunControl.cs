using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunControl : MonoBehaviour {

    public float fireRate = 0.0f;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawnPoint;
    private float nextFire = 0.0f;

    private void Awake()
    {
        
    }

    private void FixedUpdate()
    {

    }
}
