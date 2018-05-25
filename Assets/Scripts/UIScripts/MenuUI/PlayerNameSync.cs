using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameSync : MonoBehaviour
{
    Text playerNameDisplay;

    private void Awake()
    {
        playerNameDisplay = GetComponent<Text>();
    }

    private void Update()
    {
        if (playerNameDisplay.text != NetworkManager.instance.playerName)
        {
            playerNameDisplay.text = NetworkManager.instance.playerName;
        }
    }
}
