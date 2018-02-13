using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Base class for handling player network movement
 * Using Synchronize States to simulate other remote client's movement with margin error from ping latency as well
 */
[RequireComponent(typeof(Rigidbody))]
public class NetworkPlayerMovement : NetworkPlayerBase
{
    #region Public Members

    #endregion

    #region Private Members
    /// <summary>
    /// Rigidbody reference of the player
    /// </summary>
    protected Rigidbody rb;

    /// <summary>
    /// Position received over network
    /// </summary>
    private Vector3 networkPos;

    /// <summary>
    /// Rotation received over network
    /// </summary>
    private Quaternion networkRot;

    /// <summary>
    /// Timestamp of data receival
    /// </summary>
    private double lastNetworkedReceiveTime = 0;
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    /// <summary>
    /// Find the rigidbody for reference
    /// </summary>
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Since using physics property, fixedupdate makes more sense to update the network remote clients
    /// </summary>
    private void FixedUpdate()
    {
        // Does not need to update self
        if (!photonView.isMine)
        {
            UpdateNetworkPosition();
            UpdateNetworkRotation();
        }
    }
    #endregion
    /// <summary>
    /// Using Synchronizing State method to interpolate last received position and simulates the future positions based on velocity and angular velocity
    /// Taking network latency into account as well
    /// </summary>
    private void UpdateNetworkPosition()
    {
        // Figure out how much time has passed since the network receiving time with network latency
        float pingInSeconds = (float)PhotonNetwork.GetPing() * 0.001f;
        float timeSinceLastUpdate = (float)(PhotonNetwork.time - lastNetworkedReceiveTime);
        float totalTimePassed = pingInSeconds + timeSinceLastUpdate;

        // Guess the next position based on velocity and network position with given time difference
        Vector3 exterpolatedPosition = networkPos + rb.velocity * totalTimePassed;
        Vector3 predictPosition = Vector3.MoveTowards(rb.position, exterpolatedPosition, rb.velocity.magnitude * Time.deltaTime);

        // Prevent vehicle from jittering Y-axis
        if (Mathf.Abs(rb.position.y - predictPosition.y) < 1f)
        {
            predictPosition.y = rb.position.y;
        }

        // Hard snap the position if vehicle flew off way too much
        if (Vector3.Distance(rb.position, exterpolatedPosition) > 2f)
        {
            predictPosition = exterpolatedPosition;
        }

        rb.MovePosition(predictPosition);
    }

    /// <summary>
    /// Lerp the vehicle rotation towards networked rotation
    /// </summary>
    private void UpdateNetworkRotation()
    {
        rb.rotation = Quaternion.RotateTowards(rb.rotation, networkRot, 180f * Time.deltaTime);
    }

    /// <summary>
    /// Methods got called by NetworkPlayerData component to send/receive network data
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    public override void SerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(rb.position);
            stream.SendNext(rb.rotation);
            stream.SendNext(rb.velocity);
            stream.SendNext(rb.angularVelocity);
        }
        else if (stream.isReading)
        {
            // save the network position into buffer
            networkPos = (Vector3)stream.ReceiveNext();
            networkRot = (Quaternion)stream.ReceiveNext();

            // Hard snap the physics property
            rb.velocity = (Vector3)stream.ReceiveNext();
            rb.angularVelocity = (Vector3)stream.ReceiveNext();

            // Timestamp the data
            lastNetworkedReceiveTime = info.timestamp;
        }
    }
}
