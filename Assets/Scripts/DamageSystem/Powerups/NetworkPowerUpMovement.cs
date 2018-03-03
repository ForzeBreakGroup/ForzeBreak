using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(Rigidbody))]
public class NetworkPowerUpMovement : Photon.MonoBehaviour
{
    protected Rigidbody rb;
    protected float lastReceivedTime = 0;
    protected Vector3 networkPos;
    protected Quaternion networkRot;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //StartCoroutine(DestroyCoroutine());
    }

    protected virtual void FixedUpdate()
    {
        if (photonView.isMine)
        {
            Move();
        }
        else
        {

        }
    }

    protected void NetworkMove()
    {
        float pingInSeconds = (float)PhotonNetwork.GetPing() * 0.001f;
        float timeSinceLastUpdate = (float)(PhotonNetwork.time - lastReceivedTime);
        float totalTimePassed = pingInSeconds + timeSinceLastUpdate;

        // Missile will fly up towards sky first
        Vector3 exterpolatedPosition = networkPos + rb.velocity * totalTimePassed;
        Vector3 predictPosition = Vector3.MoveTowards(rb.position, exterpolatedPosition, rb.velocity.magnitude * Time.deltaTime);

        if (Vector3.Distance(rb.position, exterpolatedPosition) > 2f)
        {
            predictPosition = exterpolatedPosition;
        }
        rb.MovePosition(predictPosition);
        rb.rotation = Quaternion.RotateTowards(rb.rotation, networkRot, 180f * Time.deltaTime);
    }

    protected virtual void Move()
    {

    }

    protected void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(rb.velocity);
            stream.SendNext(rb.angularVelocity);
        }
        else if (stream.isReading)
        {
            networkPos = (Vector3)stream.ReceiveNext();
            networkRot = (Quaternion)stream.ReceiveNext();
            rb.velocity = (Vector3)stream.ReceiveNext();
            rb.angularVelocity = (Vector3)stream.ReceiveNext();

            lastReceivedTime = (float)info.timestamp;
        }
    }

    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(10);
        PhotonNetwork.Destroy(this.gameObject);
    }
}
