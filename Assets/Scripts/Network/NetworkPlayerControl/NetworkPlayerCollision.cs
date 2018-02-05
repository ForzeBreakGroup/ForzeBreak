using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collision))]
public class NetworkPlayerCollision : NetworkPlayerBase
{
    protected enum CollisionResult
    {
        Collider,
        Receiver
    };

    private void OnCollisionEnter(Collision collision)
    {
        if (PhotonNetwork.isMasterClient && collision.gameObject.tag == "Player")
        {
            CollisionResult collisionResult = CollisionEvent(collision);
            photonView.RPC("NetworkCollision", PhotonPlayer.Find(this.gameObject.GetPhotonView().ownerId), collisionResult, collision.impulse.magnitude);
        }
    }

    protected virtual CollisionResult CollisionEvent(Collision collision)
    {
        return CollisionResult.Receiver;
    }

    [PunRPC]
    private void NetworkCollision(CollisionResult collisionResult, float force)
    {
        ResolveCollision(collisionResult, force);
    }

    protected virtual void ResolveCollision(CollisionResult collisionResult, float force)
    {

    }
}
