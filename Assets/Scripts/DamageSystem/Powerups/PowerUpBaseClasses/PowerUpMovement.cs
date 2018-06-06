using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpMovement : PowerUpProjectileBase
{
    protected virtual void OnDestroy()
    {

    }

    public void DestroyPowerUpProjectile()
    {
        Debug.Log("Destroying Projectile: " + gameObject.name + " called by Player #" + NetworkManager.instance.GetLocalPlayer().GetPhotonView().ownerId);
        PhotonView.RPC("RpcDestroyPowerUpProjectile", PhotonTargets.All);
    }

    [PunRPC]
    public virtual void RpcDestroyPowerUpProjectile()
    {
        if (PhotonView.isMine)
        {
            Debug.Log("RPC Received: " + gameObject.name);
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
