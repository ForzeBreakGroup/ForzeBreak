using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : Photon.MonoBehaviour
{
    private Dictionary<PhotonPlayer, bool> playersStillAlive;

    private void Awake()
    {
        playersStillAlive = new Dictionary<PhotonPlayer, bool>();
    }

    private void OnEnable()
    {
        PhotonNetwork.OnEventCall += EvtPlayerDeathEventHandler;
        PhotonNetwork.OnEventCall += EvtAddPlayerEventHandler;
        PhotonNetwork.OnEventCall += EvtRemovePlayerEventHandler;
    }

    private void OnDisable()
    {
        PhotonNetwork.OnEventCall -= EvtPlayerDeathEventHandler;
        PhotonNetwork.OnEventCall -= EvtAddPlayerEventHandler;
        PhotonNetwork.OnEventCall -= EvtRemovePlayerEventHandler;
    }

    private void EvtAddPlayerEventHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte)ENetworkEventCode.OnAddPlayerToMatch && PhotonNetwork.isMasterClient)
        {
            Debug.Log("EvtAddPlayerEvent");
            PhotonPlayer player = PhotonPlayer.Find(senderid);

            if (!playersStillAlive.ContainsKey(player))
            {
                playersStillAlive.Add(player, true);
            }
        }
    }

    private void EvtRemovePlayerEventHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte)ENetworkEventCode.OnRemovePlayerFromMatch && PhotonNetwork.isMasterClient)
        {
            Debug.Log("EvtRemovePlayerEvent");
            PhotonPlayer player = PhotonPlayer.Find(senderid);

            if (!playersStillAlive.ContainsKey(player))
            {
                playersStillAlive.Remove(player);
            }
        }
    }

    private void EvtPlayerDeathEventHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte) ENetworkEventCode.OnPlayerDeath && PhotonNetwork.isMasterClient)
        {
            Debug.Log("Player Death Event Handler");
            PhotonPlayer sender = PhotonPlayer.Find(senderid);
            playersStillAlive[sender] = false;

            uint aliveCount = 0;
            foreach(KeyValuePair<PhotonPlayer, bool> entry in playersStillAlive)
            {
                if (entry.Value)
                {
                    ++aliveCount;
                }
            }

            if (aliveCount <= 1)
            {
                PhotonNetwork.LoadLevel("End");
            }
        }
    }
}
