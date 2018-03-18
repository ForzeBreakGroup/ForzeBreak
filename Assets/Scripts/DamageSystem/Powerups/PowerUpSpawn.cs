using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Powerup spawn point to spawn assigned powerup prefabs
 * 
 */
[System.Serializable]
public struct PowerUpSpawnData
{
    /// <summary>
    /// Powerup prefab
    /// </summary>
    public GameObject powerUpItem;

    /// <summary>
    /// Possibility rate of spawning
    /// </summary>
    [Range(0, 1)]
    public float powerUpSpawnRate;
}

public class PowerUpSpawn : Photon.MonoBehaviour
{
    #region Members

    /// <summary>
    /// List of powerup spawning object data
    /// </summary>
    [SerializeField]
    private PowerUpSpawnData[] powerUpSpawnData;

    /// <summary>
    /// Internal refernece to the spawned object
    /// </summary>
    private GameObject collectible;

    /// <summary>
    /// Adjustable cooldown before next spawn
    /// </summary>
    [SerializeField]
    private float cooldown = 15.0f;

    /// <summary>
    /// Internal cooldown counter
    /// </summary>
    private float elapsedTime = 0;

    /// <summary>
    /// Boolean indicate if the cooldown is enabled
    /// </summary>
    private bool inCooldown = false;
    #endregion

    #region Methods
    /// <summary>
    /// Spawn collectible when awaked
    /// </summary>
    private void Awake()
    {
        //SpawnCollectible();
    }

    /// <summary>
    /// Register to photon events
    /// </summary>
    private void OnEnable()
    {
        PhotonNetwork.OnEventCall += OnPowerUpCollectedHandler;
    }

    /// <summary>
    /// Deregister the photon events
    /// </summary>
    private void OnDisable()
    {
        PhotonNetwork.OnEventCall -= OnPowerUpCollectedHandler;
    }

    /// <summary>
    /// Spawn one of pre-assigned collectible gameobject based on the possibility rating
    /// </summary>
    public void SpawnCollectible()
    {
        // Only masterclient will handle the logic
        if (PhotonNetwork.isMasterClient)
        {
            // Highest value tracker
            float highestValue = 0f;

            // Index of highest value
            int highestValueIndex = 0;

            // Generate a random number for each possible power up
            for (int i = 0; i < powerUpSpawnData.Length; ++i)
            {
                // Value = Random number [1, 10000] * spawn rate multiplier
                // Will spawn the highest valued collectible
                float value = Random.Range(1, 10000) * powerUpSpawnData[i].powerUpSpawnRate;

                if (value > highestValue)
                {
                    highestValue = value;
                    highestValueIndex = i;
                }
            }

            // Destroy previous collectible if already existed
            if (collectible)
            {
                PhotonNetwork.Destroy(collectible);
            }
            collectible = PhotonNetwork.Instantiate(powerUpSpawnData[highestValueIndex].powerUpItem.name, transform.position, Quaternion.identity, 0);
        }
    }

    /// <summary>
    /// When powerup is collected, the remote client will notify masterclient for properly destroy the object
    /// </summary>
    /// <param name="evtCode"></param>
    /// <param name="content"></param>
    /// <param name="senderid"></param>
    private void OnPowerUpCollectedHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte)ENetworkEventCode.OnPowerUpCollected)
        {
            Vector3 powerUpPos = (Vector3)content;

            if (powerUpPos == transform.position)
            {
                if (collectible != null)
                {
                    PhotonNetwork.Destroy(collectible);
                }

                elapsedTime = 0;
                inCooldown = true;
            }
        }
    }

    private void Update()
    {
        if (PhotonNetwork.isMasterClient)
        {
            elapsedTime += Time.deltaTime;
            if (inCooldown && elapsedTime >= cooldown)
            {
                inCooldown = false;
                SpawnCollectible();
            }
        }
    }

    #endregion
}
