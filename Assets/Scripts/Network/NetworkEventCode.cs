using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENetworkEventCode : byte
{
    OnAddPlayerToMatch = (byte)0x00,
    OnRemovePlayerFromMatch = (byte)0x01,
    OnPlayerDeath = (byte)0x12,
};
