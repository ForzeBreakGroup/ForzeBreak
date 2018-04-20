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

    public GameObject externalCollider;

    private GameObject target;
    private DamageSystem targetDmgSystem;

    private void OnCollisionEnter(Collision collision)
    {
        if (PowerUpData.OwnerID == -1)
        {
            Debug.LogError("Owner ID is not set properly");
        }

        GameObject collidingEntity = externalCollider;
        if (collidingEntity == null)
        {
            collidingEntity = collision.collider.transform.root.gameObject;
        }

        if (ValidateColliderEvent(collidingEntity))
        {
            CollisionEnter(collision);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PowerUpData.OwnerID == -1)
        {
            Debug.LogError("Owner ID is not set properly");
        }

        GameObject collidingEntity = externalCollider;
        if (collidingEntity == null)
        {
            collidingEntity = other.transform.root.gameObject;
        }

        if (ValidateColliderEvent(collidingEntity))
        {
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

        if (isSelf && isPlayer)
        {
            target = collider;
            if (checkPlayer)
            {
                targetDmgSystem = target.GetComponent<DamageSystem>();
            }
        }

        return (isSelf && isPlayer);
    }

    public void TransferTarget(GameObject newTarget)
    {
        target = newTarget;
        targetDmgSystem = target.GetComponent<DamageSystem>();
    }

    protected virtual void ApplyDamage()
    {
        if (targetDmgSystem != null)
        {
            targetDmgSystem.ApplyDamageForce(damage, this.transform.position + centerOffset, damageRadius);
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
