using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NetworkPlayerMovement : NetworkPlayerBase
{
    protected Rigidbody rb;
    private Vector3 networkPos;
    private Quaternion networkRot;
    private double lastNetworkedReceiveTime = 0;

    private void FixedUpdate()
    {
        if (!photonView.isMine)
        {
            UpdateNetworkPosition();
            UpdateNetworkRotation();
        }
    }

    public void AssignRigidbody()
    {
        rb = GetComponent<Rigidbody>();

    }
    private void UpdateNetworkPosition()
    {
        float pingInSeconds = (float)PhotonNetwork.GetPing() * 0.001f;
        float timeSinceLastUpdate = (float)(PhotonNetwork.time - lastNetworkedReceiveTime);
        float totalTimePassed = pingInSeconds + timeSinceLastUpdate;

        Vector3 exterpolatedPosition = networkPos + rb.velocity * totalTimePassed;
        Vector3 predictPosition = Vector3.MoveTowards(rb.position, exterpolatedPosition, rb.velocity.magnitude * Time.deltaTime);

        if (Mathf.Abs(rb.position.y - predictPosition.y) < 1f)
        {
            predictPosition.y = rb.position.y;
        }

        if (Vector3.Distance(rb.position, exterpolatedPosition) > 2f)
        {
            predictPosition = exterpolatedPosition;
        }

        rb.MovePosition(predictPosition);
    }

    private void UpdateNetworkRotation()
    {
        rb.rotation = Quaternion.RotateTowards(rb.rotation, networkRot, 180f * Time.deltaTime);
    }

    public override void SerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            Debug.Log(stream);
            Debug.Log(rb);
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
