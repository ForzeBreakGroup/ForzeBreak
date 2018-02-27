using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

public class NetworkTimer : Photon.MonoBehaviour
{
    private static NetworkTimer networkTimer;
    private Text timerText;
    [SerializeField] private float time = 180.0f;
    private float currentTime = 0;
    private bool startCounting = false;

    private void Awake()
    {
        timerText = GetComponent<Text>();
        timerText.enabled = false;
    }

    private void OnEnable()
    {
        PhotonNetwork.OnEventCall += EvtPlayerSpawningHandler;
        PhotonNetwork.OnEventCall += EvtRoundOverHandler;
    }

    private void OnDisable()
    {
        PhotonNetwork.OnEventCall -= EvtPlayerSpawningHandler;
        PhotonNetwork.OnEventCall -= EvtRoundOverHandler;
    }

    private void EvtPlayerSpawningHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte)ENetworkEventCode.OnPlayerSpawning)
        {
            timerText.enabled = true;
            startCounting = true;
            currentTime = time;

            DisplayTime();
        }
    }

    private void EvtRoundOverHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte)ENetworkEventCode.OnRoundOver)
        {
            timerText.enabled = false;
            startCounting = false;
        }
    }

    private void Update()
    {
        if (startCounting)
        {
            currentTime -= Time.deltaTime;
            DisplayTime();

            if (currentTime <= 0 && PhotonNetwork.isMasterClient)
            {
                RaiseEventOptions options = new RaiseEventOptions();
                options.Receivers = ReceiverGroup.All;
                int drawId = -1;
                PhotonNetwork.RaiseEvent((byte)ENetworkEventCode.OnRoundOver, drawId , true, options);
            }
        }
    }

    private void DisplayTime()
    {
        int min = (int)(currentTime / 60);
        int sec = (int)(currentTime % 60);

        timerText.text = string.Format("{0} : {1}", min.ToString("D2"), sec.ToString("D2"));
    }
}
