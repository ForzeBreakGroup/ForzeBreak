using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Serve as base framework for send/receive all player related collision events
 */
[RequireComponent(typeof(Collision))]
public class NetworkPlayerCollision : NetworkPlayerBase
{
    /// <summary>
    /// Enum defines player object collision result
    /// </summary>
    protected enum PlayerCollisionResult
    {
        /// <summary>
        /// The player is colliding the other as collider
        /// </summary>
        Collider,

        /// <summary>
        /// The player in collision is considered as receiver
        /// </summary>
        Receiver
    };

    
    /// <summary>
    /// Unity lifehook event when Collision happens
    /// The server-side has the authority over when the collision happens, as well as the analysis result
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
    // Host side collision check
    if (collision.transform.root.tag == "Player" && collision.transform.root.gameObject.GetPhotonView().isMine)
    {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().shakeDuration = 0.1f;
    }
}
    

    /// <summary>
    /// Callback function that child class must override, this dictates the reaction of collision
    /// </summary>
    /// <param name="collision"></param>
    /// <param name="force"></param>
    /// <param name="collisionPoint"></param>
    /// <returns></returns>
    protected virtual PlayerCollisionResult CollisionEvent(Collision collision, out float force, out Vector3 collisionPoint)
    {
        force = 0;
        collisionPoint = Vector3.zero;
        return PlayerCollisionResult.Receiver;
    }

    /// <summary>
    /// PhotonRPC method, calls to child class to resolve the collision
    /// </summary>
    /// <param name="collisionResult"></param>
    /// <param name="force"></param>
    /// <param name="contactPoint"></param>
    [PunRPC]
    protected void NetworkCollision(PlayerCollisionResult collisionResult, float force, Vector3 contactPoint)
    {
        ResolveCollision(collisionResult, force, contactPoint);
    }

    /// <summary>
    /// Virtual method for child class to implement the collision result
    /// </summary>
    /// <param name="collisionResult"></param>
    /// <param name="force"></param>
    /// <param name="point"></param>
    protected virtual void ResolveCollision(PlayerCollisionResult collisionResult, float force, Vector3 point)
    {

    }
}
