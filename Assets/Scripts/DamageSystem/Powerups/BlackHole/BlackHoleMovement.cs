using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleMovement : PowerUpMovement
{
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 15, ForceMode.Impulse);
    }
}
