using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Base class for all powerups, override OnPress, OnHold, OnRelease for handling the specific event you want
 * Awake must override from this class and calls base.Awake() for correctly getting the reticle system results
 */
public class PowerUpBase : Photon.MonoBehaviour
{
    /// <summary>
    /// Player number indicated by the vehicle data, used for distinguishing the player controllers in local mode
    /// </summary>
    public int playerNum = 0;

    /// <summary>
    /// The component offset relative to the vehicle model
    /// </summary>
    [SerializeField] protected Vector3 componentOffset;

    /// <summary>
    /// The component offset angle relative to the vehicle model
    /// </summary>
    [SerializeField] protected Vector3 componentAngle = Vector3.zero;

    /// <summary>
    /// The number of times this powerup can spawn, automatically destroy component when the limit reached
    /// </summary>
    [SerializeField] protected uint bulletCount = 1;

    /// <summary>
    /// Owner ID, derived from photon view ID
    /// </summary>
    private int ownerID = -1;

    /// <summary>
    /// Item to be spawned when key pressed
    /// </summary>
    [SerializeField] protected GameObject spawnItem;

    public virtual void AdjustModel()
    {
        enabled = transform.root.gameObject.GetPhotonView().isMine;
        playerNum = transform.root.gameObject.GetComponent<CarUserControl>().playerNum;

        transform.localPosition = componentOffset;
        transform.localRotation = Quaternion.Euler(componentAngle);
    }

    protected virtual void OnPress()
    {
        --bulletCount;
    }

    protected virtual void OnHold()
    {
    }

    protected virtual void OnRelease()
    {
        if (bulletCount <= 0)
        {
            UnloadPowerUp();
        }
    }

    protected virtual void Update()
    {
        // Get input from player
        if (Input.GetButtonDown("WeaponFire_Mouse") || Input.GetButtonDown("WeaponFire_Controller" + playerNum))
        {
            OnPress();
        }
        else if (Input.GetButtonUp("WeaponFire_Mouse") || Input.GetButtonUp("WeaponFire_Controller" + playerNum))
        {
            OnRelease();
        }
        else if (Input.GetButton("WeaponFire_Mouse") || Input.GetButton("WeaponFire_Controller" + playerNum))
        {
            OnHold();
        }
    }

    [PunRPC]
    protected void SetParent(int parentID)
    {
        NetworkPlayerVisual[] players = FindObjectsOfType<NetworkPlayerVisual>();
        GameObject target = null;

        ownerID = parentID;

        foreach(NetworkPlayerVisual p in players)
        {
            if (p.transform.root.gameObject.GetPhotonView().viewID == parentID)
            {
                target = p.transform.root.gameObject;
            }
        }

        transform.SetParent(target.transform);
    }

    protected virtual void UnloadPowerUp()
    {
        PhotonView view = transform.root.gameObject.GetPhotonView();
        if (view.isMine)
        {
            view.RPC("RemovePowerUpComponent", PhotonTargets.All, view.viewID);
        }
    }
}
