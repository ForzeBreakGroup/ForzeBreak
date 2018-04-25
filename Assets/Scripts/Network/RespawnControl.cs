using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnControl : MonoBehaviour
{
    [SerializeField]
    private float respawnCooldown = 5.0f;

    private int playerNum;

    public void StartRespawnTimer(int index)
    {
        playerNum = index - 1;
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        Debug.Log("Respawning Player #" + playerNum + " in " + respawnCooldown + " seconds");
        yield return new WaitForSeconds(respawnCooldown);
        MatchManager.instance.SpawnPlayer(playerNum);
    }
}
