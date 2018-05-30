using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusEntry : MonoBehaviour
{
    Text playerName;
    Text status;

    GameObject active;
    GameObject waiting;

    public PhotonPlayer p;

    [SerializeField]
    private Color readyColor = Color.green;

    [SerializeField]
    private Color notReadyColor = Color.red;

    private void Update()
    {
        if (p != null)
        {
            playerName.text = p.NickName;
            this.status.text = (LobbyManager.instance.playerReadyStatus[p]) ? "Ready" : "Not Ready";
            this.status.color = (LobbyManager.instance.playerReadyStatus[p]) ? readyColor : notReadyColor;
        }
    }

    private void Awake()
    {
        active = transform.Find("Active").gameObject;
        waiting = transform.Find("Waiting").gameObject;
        playerName = transform.Find("Active").Find("Name").GetComponent<Text>();
        status = transform.Find("Active").Find("Ready").GetComponent<Text>();

        DisableEntry();
    }

    public void DisableEntry()
    {
        active.SetActive(false);
        waiting.SetActive(true);

        p = null;
    }

    public void EnableEntry(PhotonPlayer player)
    {
        Debug.Log("Enabling player entry with player name: " + player.NickName);
        active.SetActive(true);
        waiting.SetActive(false);

        p = player;
    }
}
