using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCollision : PowerUpProjectileBase
{
    [SerializeField] protected float damage = 100.0f;
    [SerializeField] protected float damageRadius = 10.0f;
    [SerializeField] protected Vector3 centerOffset = Vector3.zero;

    [SerializeField] protected bool checkSelf = true;
    [SerializeField] protected bool checkPlayer = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (PowerUpData.OwnerID == -1)
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
        if (PowerUpData.OwnerID == -1)
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
        Debug.Log("Colliding with Self: " + checkSelf);
        Debug.Log("Colliding with Player: " + checkPlayer);
        bool isSelf = (checkSelf) ? collider.GetPhotonView().ownerId != PowerUpData.OwnerID : true;
        bool isPlayer = (checkPlayer) ? collider.tag == "Player" : true;
        Debug.Log(collider.GetPhotonView().ownerId);
        Debug.Log(PowerUpData.OwnerID);
        Debug.Log("Is Colldiing with Player: " + isPlayer);

        return (isSelf && isPlayer);
    }

    protected virtual void CollisionEnter(Collision collision)
    {

    }

    protected virtual void TriggerEnter(Collider other)
    {

    }
}
