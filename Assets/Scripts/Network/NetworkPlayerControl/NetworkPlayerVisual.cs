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

    private Transform canvas;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").transform;
        if (!photonView.isMine)
        {
            InitializeVehicleWithPlayerColor();
        }
    }

    public void InitializeVehicleWithPlayerColor()
    {
        Renderer[] rend = transform.Find("Model").Find("Body").GetComponentsInChildren<Renderer>();

        //Deserialize color through network
        Color c;
        if (NetworkManager.offlineMode)
        {
            c = NetworkManager.instance.GetPlayerColor(GetComponent<CarUserControl>().controllerNum - 1);
        }
        else
        {
            float[] serializeColor = PhotonPlayer.Find(photonView.ownerId).CustomProperties["Color"] as float[];
            c = new Color(serializeColor[0], serializeColor[1], serializeColor[2], serializeColor[3]);
        }

        foreach(Renderer r in rend)
        {
            r.material.color = c;
        }
    }

    [PunRPC]
    protected void AddPowerUpComponent(string powerupName, int targetID)
    {
        if (photonView.isMine && photonView.ownerId == targetID)
        {
            currentEquipped = PhotonNetwork.Instantiate(powerupName, transform.position, Quaternion.identity, 0);
            ((PowerUpComponent)currentEquipped.GetComponent(typeof(PowerUpComponent))).SetComponentParent(targetID);
            ((PowerUpComponent)currentEquipped.GetComponent(typeof(PowerUpComponent))).AdjustModel();
            InGameHUDManager.instance.UpdateWeaponIcon(powerupName);
        }
    }

    [PunRPC]
    protected void RemovePowerUpComponent(int targetID)
    {
        if (photonView.isMine && photonView.ownerId == targetID)
        {
            if (currentEquipped != null)
            {
                InGameHUDManager.instance.UpdateWeaponIcon("");

                PhotonNetwork.Destroy(currentEquipped);
                currentEquipped = null;
            }
        }
    }
}
