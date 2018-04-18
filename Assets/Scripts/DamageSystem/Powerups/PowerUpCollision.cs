using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCollision : MonoBehaviour
{
    [SerializeField] protected bool checkSelf = true;
    [SerializeField] protected bool checkPlayer = true;

    int ownerID = -1;

    public void SetOwnerId(int id)
    {
        ownerID = id;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (ownerID == -1)
        {
            Debug.LogError("Owner ID is not set properly");
        }

        bool isSelf = (checkSelf) ? collision.transform.root.gameObject.GetPhotonView().ownerId != ownerID : true;
        bool isPlayer = (checkPlayer) ? collision.transform.root.gameObject.tag == "Player" : true;

        if (isSelf && isPlayer)
        {
            CollisionEnter(collision);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ownerID == -1)
        {
            Debug.LogError("Owner ID is not set properly");
        }

        if (other.transform.root.gameObject.GetPhotonView().ownerId != ownerID)
        {
            TriggerEnter(other);
        }
    }

    protected virtual void CollisionEnter(Collision collision)
    {

    }

    protected virtual void TriggerEnter(Collider other)
    {

    }
}
