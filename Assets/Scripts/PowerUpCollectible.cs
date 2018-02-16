using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PowerUpCollectible : Photon.MonoBehaviour
{
    private Renderer rend;
    private bool powerUpCollected = false;
    private float elapsedTime = 0.0f;
    [SerializeField] private float cooldown = 5.0f;
    [SerializeField] protected string powerupName = "MissileVersion1";

    protected void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    protected void Update()
    {
        if(powerUpCollected)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= cooldown)
            {
                powerUpCollected = false;
                EnablingPowerup(!powerUpCollected);
            }
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (!powerUpCollected && other.transform.root.tag == "Player")
        {
            if (PhotonNetwork.isMasterClient)
            {
                PhotonView view = other.transform.root.gameObject.GetPhotonView();
                view.RPC("RemovePowerUpComponent", PhotonTargets.All, view.viewID);
                view.RPC("AddPowerUpComponent", PhotonTargets.All, powerupName, view.viewID);
            }

            powerUpCollected = true;
            elapsedTime = 0;

            EnablingPowerup(!powerUpCollected);
        }
    }

    private void EnablingPowerup(bool enable)
    {
        rend.enabled = enable;
    }
}
