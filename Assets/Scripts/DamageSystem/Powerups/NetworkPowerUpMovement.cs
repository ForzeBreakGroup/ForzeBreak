using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Base movement network synchronization class for all projectile powerups
 */
[RequireComponent(typeof(Rigidbody))]
public class NetworkPowerUpMovement : Photon.MonoBehaviour
{
    #region Protected Members
    /// <summary>
    /// Internal reference to the rigidbody
    /// </summary>
    protected Rigidbody rb;

    /// <summary>
    /// Timestamp for when the last synchronization time
    /// </summary>
    protected float lastReceivedTime = 0;

    /// <summary>
    /// The received position from network
    /// </summary>
    protected Vector3 networkPos;

    /// <summary>
    /// The received rotation from network
    /// </summary>
    protected Quaternion networkRot;
    #endregion

    #region Methods
    /// <summary>
    /// Finds the Rigidbody reference
    /// </summary>
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //StartCoroutine(DestroyCoroutine());
    }

    /// <summary>
    /// Unified fixed update entry point for all child class
    /// </summary>
    protected virtual void FixedUpdate()
    {
        if (photonView.isMine)
        {
            Move();
        }
    }

    /// <summary>
    /// For each inherited child class to override the movement
    /// </summary>
    protected virtual void Move()
    {

    }

    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(10);
        PhotonNetwork.Destroy(this.gameObject);
    }

    #endregion
}
