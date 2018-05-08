using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnnouncerManager : MonoBehaviour
{
    private Animator announcerAnim;
    private Text announcerText;
    private AudioSource announcerAudio;
    private AudioClip announcingClip;

    private static UIAnnouncerManager announcerManager;
    public static UIAnnouncerManager instance
    {
        get
        {
            if (!announcerManager)
            {
                announcerManager = FindObjectOfType(typeof(UIAnnouncerManager)) as UIAnnouncerManager;
                if (!announcerManager)
                {
                    Debug.LogError("UIAnnouncerManager Script must be attached to an active GameObject in scene");
                }
                else
                {
                    announcerManager.Init();
                }
            }

            return announcerManager;
        }
    }

    private void Init()
    {
        announcerAnim = GetComponent<Animator>();
        announcerText = GetComponent<Text>();
        announcerText.enabled = false;
        // announcerAudio = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (announcerText)
        {
            announcerText.enabled = true;
        }
    }

    private void OnDisable()
    {
        if (announcerText)
        {
            announcerText.enabled = false;
        }
    }

    // target id = -1 means annouce to everyone
    public void Announce(string announceText, AudioClip announceClip, int targetId = -1)
    {
        if (targetId == -1 || PhotonNetwork.player.ID == targetId)
        {
            announcerText.text = announceText;
            announcerAnim.SetTrigger("PlayAnnouncer");
        }
    }
}
