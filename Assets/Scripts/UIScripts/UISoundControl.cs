using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UISoundControl : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip onEnterSound;
    public AudioClip onConfirmSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void onHover()
    {
        audioSource.PlayOneShot(onEnterSound);
    }

    public void onConfirm()
    {
        audioSource.PlayOneShot(onConfirmSound);
    }


}