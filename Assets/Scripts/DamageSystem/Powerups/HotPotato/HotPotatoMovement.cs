using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotPotatoMovement : PowerUpMovement
{
    public int targetId { get; private set; }

    public void SetTarget(GameObject target)
    {
        this.transform.SetParent(target.transform);
    }

    public void TransferHotPotato(int targetViewId)
    {
        PhotonView.RPC("RpcTransferHotPotato", PhotonTargets.All, targetViewId);
    }

    [PunRPC]
    public void RpcTransferHotPotato(int targetOwnerId)
    {
        targetId = targetOwnerId;

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in gameObjects)
        {
            if (go.GetComponent<PhotonView>().ownerId == targetOwnerId)
            {
                GetComponent<HotPotatoMovement>().SetTarget(go);
                return;
            }
        }
    }
}
