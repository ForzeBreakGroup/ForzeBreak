using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookCollision : PowerUpCollision
{
    protected override void TriggerEnter(Collider other)
    {
        if (other.transform.root.GetComponent<PhotonView>().isMine)
        {

            Rigidbody r = GetComponent<Rigidbody>();
            r.isKinematic = true;
            r.velocity = Vector3.zero;

            GetComponent<BoxCollider>().enabled = false;
            ((HookMovement)PowerUpMovement).SetHookTarget(other.transform.root.GetComponent<PhotonView>().ownerId);
        }


    }

}
