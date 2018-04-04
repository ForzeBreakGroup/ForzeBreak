using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchLobbyUIControl : MonoBehaviour
{
    [SerializeField]
    private GameObject playerStatusEntryUI;

    private Transform playerStatusPanel; 

    private void Awake()
    {
        playerStatusPanel = transform.Find("PlayerReadyStatusPanel");
    }

    private void UpdatePlayerStatus()
    {

    }

    #region UI Events
    public void OnClickPlayerReady()
    {
        LobbyManager.instance.OnClickReady();
    }

    private void Update()
    {
    }
    #endregion
}
