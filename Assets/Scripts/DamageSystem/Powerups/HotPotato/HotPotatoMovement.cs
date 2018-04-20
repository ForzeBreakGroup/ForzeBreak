using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotPotatoMovement : PowerUpMovement
{
    private GameObject target;
    public int targetId { get; private set; }
    [SerializeField] private Vector3 posOffset = Vector3.zero;
    [SerializeField] private float transferCooldown = 1.0f;
    private float elapsedTime = 0.0f;
    private bool isTransferrable = true;

    private void Update()
    {
        // Cooldown
        if (!isTransferrable)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= transferCooldown)
            {
                isTransferrable = true;
                elapsedTime = 0.0f;
            }
        }

        // Lerp the target
        if (target != null)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, target.transform.position + posOffset, Time.deltaTime * 10);
        }
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
        this.transform.SetParent(target.transform);

        ((HotPotatoCollision)PowerUpCollision).TransferTarget(target);
    }

    public void TransferHotPotato(int targetViewId)
    {
        if (isTransferrable)
        {
            PhotonView.RPC("RpcTransferHotPotato", PhotonTargets.All, targetViewId);
        }
    }

    [PunRPC]
    public void RpcTransferHotPotato(int targetOwnerId)
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in gameObjects)
        {
            if (go.GetComponent<PhotonView>().ownerId == targetOwnerId)
            {
                targetId = targetOwnerId;
                GetComponent<HotPotatoMovement>().SetTarget(go);
                PhotonView.TransferOwnership(targetOwnerId);
                return;
            }
        }
    }
}
