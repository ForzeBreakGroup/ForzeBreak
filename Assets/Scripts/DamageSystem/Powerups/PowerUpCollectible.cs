using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PowerUpCollectible : Photon.MonoBehaviour
{
    public bool randomMode = true;
    public string powerupName;

    public bool powerUpCollected = false;
    private float elapsedTime = 0.0f;
    [SerializeField] private float cooldown = 5.0f;
    public PowerUpGrade.TierLevel powerUpTier = PowerUpGrade.TierLevel.COMMON;
    private PowerUpGrade powerUpGrade;

    protected void Awake()
    {
        powerUpGrade = new PowerUpGrade();
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (!powerUpCollected && other.transform.root.tag == "Player")
        {
            PhotonView view = other.transform.root.gameObject.GetPhotonView();
            if (view.isMine)
            {
                if(randomMode)
                {
                    powerupName = powerUpGrade.GetRandomPowerUp(powerUpTier);
                }
                view.RPC("RemovePowerUpComponent", PhotonTargets.All, view.viewID);
                view.RPC("AddPowerUpComponent", PhotonTargets.All, powerupName, view.viewID);
            }

            RaiseEventOptions options = new RaiseEventOptions();
            options.Receivers = ReceiverGroup.MasterClient;

            PhotonNetwork.RaiseEvent((byte)ENetworkEventCode.OnPowerUpCollected, transform.position, true, options);

            // Hide in remote client side to create illusion of powerup has been collected immediately, otherwise, it will have delay to wait for masterclient to destroy the object
            foreach(Transform t in transform)
            {
                Renderer rend = t.GetComponent<Renderer>();
                if (rend)
                {
                    rend.enabled = false;
                }
            }
            transform.GetComponent<Renderer>().enabled = false;
            powerUpCollected = true;
        }
    }
}
