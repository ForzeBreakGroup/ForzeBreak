using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
public class SimpleMovement : NetworkBehaviour
{
    PlayerLife pl;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pl = GetComponent<PlayerLife>();
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical) * 10;
        rb.AddForce(movement);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Falls out of bound");
        // If Player fall though death boundary
        if (other.tag == "DeathBound")
        {
            // Decrease player life
            pl.DecrementPlayerLife();
        }
    }
}
