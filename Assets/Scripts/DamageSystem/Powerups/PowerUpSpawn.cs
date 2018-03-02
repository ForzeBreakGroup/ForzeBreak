using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PowerUpSpawnData
{
    public GameObject powerUpItem;

    [Range(0, 1)]
    public float powerUpSpawnRate;
}

public class PowerUpSpawn : Photon.MonoBehaviour
{
    [SerializeField]
    private PowerUpSpawnData[] powerUpSpawnData;

    private GameObject collectible;

    [SerializeField]
    private float cooldown = 15.0f;
    private float elapsedTime = 0;
    private bool inCooldown = false;

    private void Awake()
    {
        SpawnCollectible();
    }

    private void OnEnable()
    {
        PhotonNetwork.OnEventCall += OnPowerUpCollectedHandler;
        PhotonNetwork.OnEventCall += OnRoundOverHandler;
    }

    private void OnDisable()
    {
        PhotonNetwork.OnEventCall -= OnPowerUpCollectedHandler;
        PhotonNetwork.OnEventCall -= OnRoundOverHandler;
    }

    private void SpawnCollectible()
    {
        if (PhotonNetwork.isMasterClient)
        {
            float[] randValues = new float[powerUpSpawnData.Length];
            float highestValue = 0f;
            int highestValueIndex = 0;

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

            collectible = PhotonNetwork.Instantiate(powerUpSpawnData[highestValueIndex].powerUpItem.name, transform.position, Quaternion.identity, 0);
            Debug.Log(collectible);
        }
    }

    private void OnRoundOverHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte)ENetworkEventCode.OnRoundOver)
        {
            elapsedTime = cooldown;
            inCooldown = true;
        }
    }

    private void OnPowerUpCollectedHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte)ENetworkEventCode.OnPowerUpCollected && collectible != null)
        {
            Vector3 powerUpPos = (Vector3)content;

            if (powerUpPos == transform.position)
            {
                PhotonNetwork.Destroy(collectible);
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
}
