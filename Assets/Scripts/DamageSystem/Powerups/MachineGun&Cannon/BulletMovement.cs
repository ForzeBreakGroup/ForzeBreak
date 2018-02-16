using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : NetworkPowerUpMovement
{
    
    public float velocity = 100f;
    public float explosionRadius = 2.0f;
    public float explosionForce = 1000f;
    
    protected override void Awake()
    {
        base.Awake();
    }
    
    protected override void Move()
    {
        base.Move();

        rb.velocity = velocity * transform.forward;

    }

    private void OnCollisionEnter(Collision collision)
    {

        PhotonNetwork.Instantiate("Explosion1", transform.position, Quaternion.identity,0);
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        PhotonNetwork.Destroy(gameObject);

        
        //foreach (Collider c in colliders)
        //{
        //    Rigidbody r = c.transform.root.GetComponent<Rigidbody>();
        //    if (r != null)
        //    {
        //        r.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        //    }
        //}


    }
}
