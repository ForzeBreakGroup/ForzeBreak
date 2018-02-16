using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Served as base framework for networking player's visual effects across to other remote clients
 */
public class NetworkPlayerVisual : NetworkPlayerBase
{
    protected GameObject currentEquipped;

    private void Awake()
    {
        if (!photonView.isMine)
        {
            InitializeVehicleWithPlayerColor();
        }
    }

    public void InitializeVehicleWithPlayerColor()
    {
        Renderer[] rend = transform.Find("Model").Find("Body").GetComponentsInChildren<Renderer>();

        //Deserialize color through network
        Debug.Log("OwnerId: " + photonView.ownerId + ", viewID: " + photonView.viewID);
        float[] serializeColor = PhotonPlayer.Find(photonView.ownerId).CustomProperties["Color"] as float[];
        Color c = new Color(serializeColor[0], serializeColor[1], serializeColor[2], serializeColor[3]);

        foreach(Renderer r in rend)
        {
            r.material.color = c;
        }
    }

    [PunRPC]
    protected void AddPowerUpComponent(string powerupName, int targetID)
    {
        if (photonView.isMine && photonView.viewID == targetID)
        {
            currentEquipped = PhotonNetwork.Instantiate(powerupName, transform.position, Quaternion.identity, 0);

            currentEquipped.GetPhotonView().RPC("SetParent", PhotonTargets.All, photonView.viewID);
            ((PowerUpBase)currentEquipped.GetComponent(typeof(PowerUpBase))).AdjustModel();
        }
    }

    [PunRPC]
    protected void RemovePowerUpComponent(int targetID)
    {
        if (photonView.isMine && photonView.viewID == targetID)
        {
            if (currentEquipped != null)
            {
                PhotonNetwork.Destroy(currentEquipped);
                currentEquipped = null;
            }
        }
    }
}
