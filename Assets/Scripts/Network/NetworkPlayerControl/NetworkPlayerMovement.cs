using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NetworkPlayerMovement : NetworkPlayerBase
{
    private Rigidbody rb;
    private Vector3 networkPos;
    private Quaternion networkRot;
    private double lastNetworkedReceiveTime = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (photonView.isMine)
        {
            Vector3 movement = new Vector3(NetworkPlayerInput.horizontalAxis, rb.velocity.y, NetworkPlayerInput.verticalAxis);
            rb.velocity = movement;
        }
        else
        {
            UpdateNetworPosition();
            UpdateNetworkRotation();
        }
    }

    private void UpdateNetworkRotation()
    {
        float pingInSeconds = (float)PhotonNetwork.GetPing() * 0.001f;
        float timeSinceLastUpdate = (float)(PhotonNetwork.time - lastNetworkedReceiveTime);
        float totalTimePassed = pingInSeconds + timeSinceLastUpdate;

        Vector3 exterpolatedPosition = networkPos + rb.velocity * totalTimePassed;
        Vector3 predictPosition = Vector3.MoveTowards(rb.position, exterpolatedPosition, rb.velocity.magnitude * Time.deltaTime);
        if (Vector3.Distance(rb.position, exterpolatedPosition) > 2f)
        {
            predictPosition = exterpolatedPosition;
        }
        rb.position = predictPosition;
    }

    private void UpdateNetworPosition()
    {
        rb.rotation = Quaternion.RotateTowards(rb.rotation, networkRot, 180f * Time.deltaTime);
    }

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
            networkPos = (Vector3)stream.ReceiveNext();
            networkRot = (Quaternion)stream.ReceiveNext();
            rb.velocity = (Vector3)stream.ReceiveNext();
            rb.angularVelocity = (Vector3)stream.ReceiveNext();
            lastNetworkedReceiveTime = info.timestamp;
        }
    }
}
