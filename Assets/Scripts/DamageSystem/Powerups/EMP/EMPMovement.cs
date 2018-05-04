using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPMovement : PowerUpMovement
{
    private Rigidbody rb;

    [SerializeField]
    private float existenceDuration = 3.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 15, ForceMode.Impulse);
        StartCoroutine(DestroyEMPAfterDelay());
    }

    IEnumerator DestroyEMPAfterDelay()
    {
        yield return new WaitForSeconds(existenceDuration);
        DestroyPowerUpProjectile();
    }
}
