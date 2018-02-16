using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletControl : MonoBehaviour {
    
    public float velocity = 100f;
    public float explosionRadius = 2.0f;
    
	void FixedUpdate () {
        GetComponent<Rigidbody>().velocity = velocity * transform.forward;
	}

    private void OnCollisionEnter(Collision collision)
    {
        //Instantiate(bulletEffecte, transform.position, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        Destroy(gameObject);
        foreach (Collider c in colliders)
        {
            Rigidbody r = c.GetComponent<Rigidbody>();
            if (r != null)
            {
                r.AddExplosionForce(1000, transform.position, 3);
            }
        }


    }
}
