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
[System.Serializable]
public struct ComponentOffset
{
    public string vehicleName;
    public Vector3 positionOffset;
    public Vector3 angleOffset;
};

public class PowerUpComponent : Photon.MonoBehaviour
{
    /// <summary>
    /// Player number indicated by the vehicle data, used for distinguishing the player controllers in local mode
    /// </summary>
    public int playerNum = 0;

    [SerializeField]
    private ComponentOffset[] m_componentOffset;

    /// <summary>
    /// The number of times this powerup can spawn, automatically destroy component when the limit reached
    /// </summary>
    [SerializeField] protected int capacity = 1;

    /// <summary>
    /// Owner ID, derived from photon view ID
    /// </summary>
    [SerializeField] protected int ownerID = -1;

    /// <summary>
    /// Item to be spawned when key pressed
    /// </summary>
    [SerializeField] protected GameObject spawnItem;

    protected virtual void Awake()
    {
        enabled = GetComponent<PhotonView>().isMine;
    }

    public virtual string AdjustModel()
    {
        string vehicleModel = transform.root.name.Replace("(Clone)", "");
        enabled = transform.root.gameObject.GetPhotonView().isMine;
        playerNum = transform.root.gameObject.GetComponent<CarUserControl>().playerNum;

        foreach(ComponentOffset cmp in m_componentOffset)
        {
            if (cmp.vehicleName == vehicleModel)
            {
                transform.localPosition = cmp.positionOffset;
                transform.localRotation = Quaternion.Euler(cmp.angleOffset);
            }
        }

        //transform.localPosition = componentOffset;
        // localRotation = Quaternion.Euler(componentAngle);

        return vehicleModel;
    }

    protected virtual void OnPress()
    {
        if (spawnItem != null)
        {
            DecreaseCapacity();
            GameObject spawnedItem = PhotonNetwork.Instantiate(spawnItem.name, transform.position, transform.rotation, 0);
            ((PowerUpData)spawnedItem.GetComponent(typeof(PowerUpData))).SetOwnerId(this.ownerID);
        }
    }

    protected virtual void OnHold()
    {
    }

    protected virtual void OnRelease()
    {
    }

    protected virtual void Update()
    {
        // Unloads powerup
        if (capacity <= 0)
        {
            UnloadPowerUp();
            return;
        }

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

    public virtual void SetComponentParent(int parentID)
    {
        this.gameObject.GetPhotonView().RPC("RpcSetComponentParent", PhotonTargets.All, parentID);
    }

    [PunRPC]
    protected void RpcSetComponentParent(int parentID)
    {
        NetworkPlayerVisual[] players = FindObjectsOfType<NetworkPlayerVisual>();
        GameObject target = null;

        ownerID = parentID;

        foreach(NetworkPlayerVisual p in players)
        {
            if (p.transform.root.gameObject.GetPhotonView().ownerId == parentID)
            {
                target = p.transform.root.gameObject;
            }
        }

        transform.SetParent(target.transform);
    }

    public void DecreaseCapacity()
    {
        photonView.RPC("RpcDecreaseCapacity", PhotonTargets.All);
    }

    [PunRPC]
    public void RpcDecreaseCapacity()
    {
        --capacity;
    }

    protected virtual void UnloadPowerUp()
    {
        PhotonView view = transform.root.gameObject.GetPhotonView();
        if (view.isMine)
        {
            view.RPC("RemovePowerUpComponent", PhotonTargets.All, view.ownerId);
        }
    }
}
