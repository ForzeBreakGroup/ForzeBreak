using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpMovement : PowerUpProjectileBase
{
    protected virtual void OnDestroy()
    {

    }

    protected virtual void Start()
    {
        // Debug.Log("Instatiated PowerUp Projectile: " + gameObject.name + " with PhotonView ID: " + GetComponent<PhotonView>().viewID);
    }

    public void DestroyPowerUpProjectile()
    {
         PhotonView.RPC("RpcDestroyPowerUpProjectile", PhotonTargets.All);
    }

    [PunRPC]
    public virtual void RpcDestroyPowerUpProjectile()
    {
        if (photonView.isMine)
        {
            Debug.Log("RPC Received: " + gameObject.name);
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
