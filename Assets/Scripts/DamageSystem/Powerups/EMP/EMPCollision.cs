using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPCollision : PowerUpCollision
{
    [SerializeField]
    private float EMPRadius = 20;

    Collider empCollider;
    Rigidbody rb;

    [SerializeField]
    private GameObject EMPParalysisVFX;

    [SerializeField]
    private float paralysisDuration = 1.0f;

    private bool isInEffect = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        empCollider = GetComponent(typeof(Collider)) as Collider;
    }

    protected override void CollisionEnter(Collision collision)
    {
        // Make the black hole as kinetic
        rb.useGravity = false;
        rb.isKinematic = true;

        // Change the collider to trigger
        empCollider.isTrigger = true;
        ((SphereCollider)empCollider).radius = EMPRadius;
        PlayVFX();
        GetComponent<PowerupSound>().PlaySound(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.tag == "Player")
        {
            // Remove the powerup holding
            PhotonView view = other.transform.root.gameObject.GetPhotonView();
            view.RPC("RemovePowerUpComponent", PhotonTargets.All, view.ownerId);

            // Apply paralysis effect
            Instantiate(EMPParalysisVFX, other.transform).GetComponent<EMPParalysisEffect>().DestroyAfterDuration(paralysisDuration);
        }
    }
}
