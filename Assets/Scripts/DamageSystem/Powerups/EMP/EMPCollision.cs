using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPCollision : PowerUpCollision
{
    [SerializeField]
    private float activationDelay = 0.5f;

    [SerializeField]
    private float disableDuration = 5f;

    private SphereCollider triggerRange;
    private BoxCollider landingCollider;

    private void Awake()
    {
        triggerRange = GetComponent<SphereCollider>();
        landingCollider = GetComponent<BoxCollider>();

        // Disable trigger range collider until it lands
        triggerRange.enabled = false;
    }

    protected override void CollisionEnter(Collision collision)
    {
        base.CollisionEnter(collision);
        GetComponent<Rigidbody>().isKinematic = true;

        StartCoroutine(ActivateEMP());
    }

    protected override void TriggerEnter(Collider other)
    {
        base.TriggerEnter(other);

        Transform otherRoot = other.transform.root;
        if (otherRoot.tag == "Player" && otherRoot.gameObject.GetPhotonView().isMine)
        {
            ApplyDamage();
            otherRoot.GetComponent<CarUserControl>().DisableCarControl(disableDuration);
        }
    }

    IEnumerator ActivateEMP()
    {
        landingCollider.enabled = false;
        checkPlayer = true;
        yield return new WaitForSeconds(activationDelay);
        triggerRange.enabled = true;
    }
}
