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
    private void Awake()
    {
        if (!photonView.isMine)
        {
            InitializeVehicleWithPlayerColor();
        }
    }

    public void InitializeVehicleWithPlayerColor()
    {
        Material mat = transform.Find("Model").Find("Tank_Body").GetComponent<Renderer>().material;

        //Deserialize color through network
        Debug.Log("OwnerId: " + photonView.ownerId + ", viewID: " + photonView.viewID);
        float[] serializeColor = PhotonPlayer.Find(photonView.ownerId).CustomProperties["Color"] as float[];
        Color c = new Color(serializeColor[0], serializeColor[1], serializeColor[2], serializeColor[3]);

        mat.color = c;
    }
}
