using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCollision : PowerUpProjectileBase, IComponentCollision
{
    [SerializeField] protected float damage = 100.0f;
    [SerializeField] protected float damageRadius = 10.0f;
    [SerializeField] protected Vector3 centerOffset = Vector3.zero;

    [SerializeField] protected bool checkSelf = true;
    [SerializeField] protected bool checkPlayer = true;

    public GameObject otherCollider;
    public DamageSystem otherDmgSystem;

    private void OnCollisionEnter(Collision collision)
    {
        if (PowerUpData.OwnerID == -1)
        {
            Debug.LogError("Owner ID is not set properly");
        }

        if (ValidateColliderEvent(otherCollider))
        {
            Debug.Log("PowerUpCollision Validated");
            // Only triggered if the power up is not part of vehicle
            if (otherCollider == null)
            {
                otherCollider = collision.collider.transform.root.gameObject;
                otherDmgSystem = otherCollider.GetComponent<DamageSystem>();
            }

            CollisionEnter(collision);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PowerUpData.OwnerID == -1)
        {
            Debug.LogError("Owner ID is not set properly");
        }

        if (ValidateColliderEvent(otherCollider))
        {
            // Only triggered if the power up is not part of vehicle
            if (otherCollider == null)
            {
                otherCollider = other.transform.root.gameObject;
                otherDmgSystem = otherCollider.GetComponent<DamageSystem>();
            }

            TriggerEnter(other);
        }
    }

    private bool ValidateColliderEvent(GameObject collider)
    {
        bool isSelf = true;
        if (collider.GetPhotonView() != null)
        {
            isSelf = (checkSelf) ? collider.GetPhotonView().ownerId != PowerUpData.OwnerID : true;
        }
        bool isPlayer = (checkPlayer) ? collider.tag == "Player" : true;

        return (isSelf && isPlayer);
    }

    protected virtual void ApplyDamage()
    {
        if (otherDmgSystem != null)
        {
            otherDmgSystem.ApplyDamageForce(damage, this.transform.position + centerOffset, damageRadius);
        }
    }

    protected virtual void CollisionEnter(Collision collision)
    {

    }

    protected virtual void TriggerEnter(Collider other)
    {

    }

    public virtual void ComponentCollision(Collision collision)
    {
        foreach(ContactPoint cp in collision.contacts)
        {
            if (cp.otherCollider == GetComponent<Collider>())
            {
                OnCollisionEnter(collision);
                return;
            }
        }
    }

    public virtual void ComponentTrigger(Collider other)
    {
        OnTriggerEnter(other);
    }
}
