﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleMovement : PowerUpMovement
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 12, ForceMode.Impulse);
    }

    public void EnableBlackHole()
    {
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
