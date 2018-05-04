using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleMovement : PowerUpMovement
{
    private Rigidbody rb;

    [SerializeField]
    private GameObject blackHoleEffect;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 15, ForceMode.Impulse);
    }

    public void EnableBlackHole()
    {
        Instantiate(blackHoleEffect, transform);
        StartCoroutine(DisableBlackHole());
    }

    IEnumerator DisableBlackHole()
    {
        yield return new WaitForSeconds(5);
        ((BlackHoleCollision)PowerUpCollision).DisableBlackHoleEffect();
        StartCoroutine(DestroyBlackHole());
    }

    IEnumerator DestroyBlackHole()
    {
        yield return new WaitForSeconds(5);
        DestroyPowerUpProjectile();
    }
}
