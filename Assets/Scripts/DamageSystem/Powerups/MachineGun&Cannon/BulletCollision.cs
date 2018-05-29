using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : PowerUpCollision
{
    [SerializeField]
    private GameObject explosion;

    private void Awake()
    {
        if (explosion == null)
        {
            Debug.Log("Explosion Prefab required");
        }
    }

    protected override void CollisionEnter(Collision collision)
    {
        base.CollisionEnter(collision);

        GetComponent<PowerupSound>().PlaySound(0);
        GameObject go = PhotonNetwork.Instantiate(explosion.name, transform.position, Quaternion.identity, 0);
        ((PowerUpData)go.GetComponent(typeof(PowerUpData))).SetOwnerId(PowerUpData.OwnerID);
        PowerUpMovement.DestroyPowerUpProjectile();
    }
}
