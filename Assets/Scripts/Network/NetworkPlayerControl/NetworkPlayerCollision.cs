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
        // Host side collision check
        if (collision.gameObject.tag == "Player" && PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnCollisionEvent Triggered by Player: " + photonView.viewID);
            float force;
            Vector3 point;
            CollisionResult collisionResult = CollisionEvent(collision, out force, out point);
            photonView.RPC("NetworkCollision", PhotonPlayer.Find(gameObject.GetPhotonView().ownerId), collisionResult, force, point);
        }
    }

    protected virtual CollisionResult CollisionEvent(Collision collision, out float force, out Vector3 collisionPoint)
    {
        force = 0;
        collisionPoint = Vector3.zero;
        return CollisionResult.Receiver;
    }

    [PunRPC]
    protected void NetworkCollision(CollisionResult collisionResult, float force, Vector3 contactPoint)
    {
        ResolveCollision(collisionResult, force, contactPoint);
    }

    protected virtual void ResolveCollision(CollisionResult collisionResult, float force, Vector3 point)
    {

    }
}
