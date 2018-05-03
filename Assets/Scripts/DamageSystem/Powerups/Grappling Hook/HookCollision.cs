using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookCollision : PowerUpCollision
{
    

    protected override void CollisionEnter(Collision collision)
    {
        if(collision.transform.root.GetComponent<PhotonView>().isMine)
        {

            Rigidbody r = GetComponent<Rigidbody>();
            r.isKinematic = true;
            r.velocity = Vector3.zero;

            GetComponent<BoxCollider>().enabled = false;
            ((HookMovement)PowerUpMovement).SetHookTarget(collision.transform.root.GetComponent<PhotonView>().ownerId);
        }
    }
}
