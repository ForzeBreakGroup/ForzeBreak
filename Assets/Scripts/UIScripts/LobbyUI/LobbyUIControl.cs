using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIControl : MonoBehaviour
{
    private Transform playerStatusPanel;

    PlayerStatusEntry [] entries;

    private void Awake()
    {
        LobbyManager.LobbyManagerPlayerJoinLobbyCallbackFunc = EvtOnPlayerJoinedRoomHandler;
        LobbyManager.LobbyManagerPlayerLeftLobbyCallbackFunc = EvtOnPlayerLeftRoomHandler;

        entries = transform.GetComponentsInChildren<PlayerStatusEntry>();
        playerStatusPanel = transform.Find("PlayerStatus");
    }

    private void EvtOnPlayerJoinedRoomHandler(PhotonPlayer player)
    {
        foreach (PlayerStatusEntry entry in entries)
        {
            if (entry.p == null)
            {
                entry.EnableEntry(player);
                break;
            }
        }
    }

    private void EvtOnPlayerLeftRoomHandler(PhotonPlayer player)
    {
        foreach(PlayerStatusEntry entry in entries)
        {
            if (entry.p == player)
            {
                entry.DisableEntry();
                break;
            }
        }
    }

    #region OnClick Event
    public void OnClickPlayerReady()
    {
        LobbyManager.instance.OnPlayerClickReady();
        NetworkManager.instance.selectedVehicleName = VehicleSelectionControl.instance.GetSelectVehicle();
    }

    public void OnClickQuitLobby()
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion
}
