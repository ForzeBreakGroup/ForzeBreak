using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomNameInput : MonoBehaviour
{
    private InputField ifield;

    private void OnEnable()
    {
        ifield = GetComponent<InputField>();
    }

    public void OnEditEndHandler()
    {
        // Adds custom player nickname
        if (!NetworkManager.offlineMode)
        {
            PhotonNetwork.player.NickName = ifield.text;
        }
    }
}
