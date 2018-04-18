using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCollision : PowerUpProjectileBase
{
    [SerializeField] protected bool checkSelf = true;
    [SerializeField] protected bool checkPlayer = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (PowerUpData.ownerId == -1)
        {
            Debug.LogError("Owner ID is not set properly");
        }

        if (ValidateColliderEvent(collision.transform.root.gameObject))
        {
            CollisionEnter(collision);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PowerUpData.ownerId == -1)
        {
            Debug.LogError("Owner ID is not set properly");
        }

        if (ValidateColliderEvent(other.transform.root.gameObject))
        {
            TriggerEnter(other);
        }
    }

    private bool ValidateColliderEvent(GameObject collider)
    {
        bool isSelf = (checkSelf) ? collider.GetPhotonView().ownerId != PowerUpData.ownerId : true;
        bool isPlayer = (checkPlayer) ? collider.tag == "Player" : true;

        return (isSelf && isPlayer);
    }

    protected virtual void CollisionEnter(Collision collision)
    {

    }

    protected virtual void TriggerEnter(Collider other)
    {

    }
}
