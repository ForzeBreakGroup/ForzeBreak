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
    [SerializeField]
    protected GameObject collisionEffect;
    [SerializeField]
    protected GameObject forzebreakEffect;

    [SerializeField]
    protected float collisionCooldown = 0.7f;
    private float elapsedTime = 0.0f;

    public int lastReceivedDamageFrom;
    public string receivedDamageItem = "null";
	public bool appliedDamage;

    [FMODUnity.EventRef]
    [SerializeField]
    private string explosionSoundref;

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

    protected virtual void Awake()
    {
        //Debug.Log(photonView.ownerId);
        lastReceivedDamageFrom = photonView.ownerId;
		appliedDamage = false;
    }

	protected virtual void Update()
    {
        elapsedTime -= Time.deltaTime;
        elapsedTime = Mathf.Clamp(elapsedTime, 0, collisionCooldown);
    }


    /// <summary>
    /// Unity lifehook event when Collision happens
    /// The server-side has the authority over when the collision happens, as well as the analysis result
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // Handles collision effects of self
        if (photonView.isMine && collision.gameObject.tag == "Player")
        {
            lastReceivedDamageFrom = collision.transform.root.GetComponent<PhotonView>().ownerId;
            receivedDamageItem = "vehicle";


            appliedDamage = true;

            // Validates the collision timer
            if (elapsedTime <= 0)
            {
                elapsedTime = collisionCooldown;
            }
            else
            {
                return;
            }

            ScreenCapturer.instance.ScreenCap();

            PlayCollisionEffect(collision.contacts[0].point);

            // Look for opponent's power up collider components, then execute those collision scripts
            Component[] powerupColliders = collision.gameObject.GetComponentsInChildren(typeof(PowerUpCollision));
            foreach (Component cp in powerupColliders)
            {
                PowerUpCollision puc = cp as PowerUpCollision;
                puc.externalCollider = this.gameObject;
                puc.ComponentCollision(collision);
            }
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

    protected void PlayCollisionEffect(Vector3 location)
    {
        CameraShake.Shake();

        Instantiate(forzebreakEffect, location, Quaternion.identity);
        Instantiate(collisionEffect, location, Quaternion.identity);
        FMODUnity.RuntimeManager.PlayOneShot(explosionSoundref, location);
    }
}
