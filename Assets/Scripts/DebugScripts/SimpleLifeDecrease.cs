using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerLife))]
public class SimpleLifeDecrease : NetworkBehaviour
{
    PlayerLife playerLifeScript;

    private void Start()
    {
        playerLifeScript = GetComponent<PlayerLife>();
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyUp(KeyCode.Space))
        {
            playerLifeScript.DecrementPlayerLife();
        }
    }
}
