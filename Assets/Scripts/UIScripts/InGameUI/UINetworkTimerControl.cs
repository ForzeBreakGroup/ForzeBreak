using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

public class UINetworkTimerControl : UIControl
{
    private Text timerText;
    [SerializeField] private float time = 10.0f;
    private float currentTime = 0;
    private bool startCounting = false;

    private void Awake()
    {
        timerText = GetComponent<Text>();
        timerText.enabled = false;
        currentTime = time;
    }

    private void DisplayTime()
    {
        int min = (int)(currentTime / 60);
        int sec = (int)(currentTime % 60);

        timerText.text = string.Format("{0} : {1}", min.ToString("D2"), sec.ToString("D2"));
    }

    public override void EnableUIControl()
    {
        base.EnableUIControl();
        timerText.enabled = true;
    }

    protected override void UpdateUIControl()
    {
        base.UpdateUIControl();
        currentTime -= Time.deltaTime;
        DisplayTime();

        if (currentTime <= 0 && PhotonNetwork.isMasterClient)
        {
            MatchManager.instance.MatchRoundOver();
        }
    }
}
