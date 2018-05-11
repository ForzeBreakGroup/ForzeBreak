using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleCollision : PowerUpCollision
{
    [SerializeField]
    private float blackHoleRadius = 10;

    [SerializeField]
    private float blackHoleMaxPull = 10000f;

    Collider blackHoleCollider;
    Rigidbody rb;

    private bool isInEffect = true;

    List<DamageSystem> playerInEffect; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        blackHoleCollider = GetComponent(typeof(Collider)) as Collider;
        playerInEffect = new List<DamageSystem>();
    }

    protected override void CollisionEnter(Collision collision)
    {
        // Make the black hole as kinetic
        rb.useGravity = false;
        rb.isKinematic = true;

        // Change the collider to trigger
        blackHoleCollider.isTrigger = true;
        ((SphereCollider)blackHoleCollider).radius = blackHoleRadius;
        ((BlackHoleMovement)PowerUpMovement).EnableBlackHole();
    }

    private void OnTriggerEnter(Collider other)
    {
        DamageSystem dmgSystem = other.transform.root.GetComponent<DamageSystem>();
        if (dmgSystem != null && !playerInEffect.Contains(dmgSystem))
        {
            playerInEffect.Add(dmgSystem);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DamageSystem dmgSystem = other.transform.root.GetComponent<DamageSystem>();
        if (dmgSystem != null && playerInEffect.Contains(dmgSystem))
        {
            playerInEffect.Remove(dmgSystem);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isInEffect)
        {
            // Blackhole ignores environment objects
            if (other.tag != "Environment")
            {
                Rigidbody otherRb = other.transform.root.GetComponent<Rigidbody>();
                // Apply constant pull force base on the distance between blackhole and gameobject
                if (otherRb != null)
                {
                    // Calculate Distance
                    float forceRatio = 1 - Vector3.Distance(transform.position, other.transform.position) / blackHoleRadius;
                    otherRb.AddForce((transform.position - otherRb.position).normalized * blackHoleMaxPull * forceRatio, ForceMode.Acceleration);
                }
            }
        }
    }

    public void DisableBlackHoleEffect()
    {
        isInEffect = false;
        ApplyExplosionDamage();
    }

    private void ApplyExplosionDamage()
    {
        foreach(DamageSystem dmgSystem in playerInEffect)
        {
            Debug.Log(dmgSystem.gameObject.name);
            dmgSystem.ApplyDamageForce(damage, transform.position, blackHoleRadius, PowerUpData.OwnerID);
        }
    }
}
