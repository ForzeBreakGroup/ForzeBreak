using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawnManager : MonoBehaviour
{
    private PowerUpSpawn[] powerUpSpawnPoints;

    private static PowerUpSpawnManager powerUpSpawnManager;
    public static PowerUpSpawnManager instance
    {
        get
        {
            if (!powerUpSpawnManager)
            {
                powerUpSpawnManager = FindObjectOfType<PowerUpSpawnManager>();
                if (!powerUpSpawnManager)
                {
                    Debug.LogError("PowerUpSpawnManager script must be attached to an active gameobject in scene");
                }
                else
                {
                    powerUpSpawnManager.Init();
                }
            }

            return powerUpSpawnManager;
        }
    }

    private void Init()
    {
        powerUpSpawnPoints = FindObjectsOfType<PowerUpSpawn>();
    }

    public void SpawnPowerUp()
    {
        if (PhotonNetwork.isMasterClient)
        {
            foreach (PowerUpSpawn spawnPoint in powerUpSpawnPoints)
            {
                spawnPoint.SpawnCollectible();
            }
        }
    }
}
