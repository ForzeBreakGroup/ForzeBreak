using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIControl : MonoBehaviour
{
    [SerializeField]
    private GameObject playerStatusEntry;

    private Transform playerStatusPanel;

    private void Awake()
    {
        LobbyManager.playerListUpdateCallbackFunc = UpdatePlayerList;
        playerStatusPanel = transform.Find("PlayerStatus");

        if (PhotonNetwork.isMasterClient)
        {
            EvtOnPlayerJoinedRoomHandler();
        }
    }

    private void OnEnable()
    {
        EventManager.StartListening("EvtOnPlayerJoinedRoom", EvtOnPlayerJoinedRoomHandler);
    }

    private void OnDisable()
    {
        EventManager.StopListening("EvtOnPLayerJoinedRoom", EvtOnPlayerJoinedRoomHandler);
    }

    private void EvtOnPlayerJoinedRoomHandler()
    {
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        // Remove all player status
        foreach(Transform t in playerStatusPanel)
        {
            DestroyObject(t.gameObject);
        }

        foreach(KeyValuePair<PhotonPlayer, bool> entry in LobbyManager.instance.playerReadyStatus)
        {
            GameObject e = Instantiate(playerStatusEntry, playerStatusPanel);
            e.GetComponent<PlayerStatusEntry>().UpdateStatus(entry.Key.NickName, entry.Value);
        }
    }

    #region OnClick Event
    public void OnClickPlayerReady()
    {
        LobbyManager.instance.OnPlayerClickReady();
        NetworkManager.instance.selectedVehicleName = VehicleSelectionControl.instance.GetSelectVehicle();
    }
    #endregion
}
