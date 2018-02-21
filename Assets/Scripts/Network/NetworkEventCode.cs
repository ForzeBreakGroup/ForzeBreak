using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Dedicated enum that defines each custom Photon event using byte
 */
public enum ENetworkEventCode : byte
{
    /// <summary>
    /// Event raised by Host when a new player joins the room
    /// </summary>
    OnAddPlayerToMatch = (byte)0x00,

    /// <summary>
    /// Event raised by Host when a player left the room
    /// </summary>
    OnRemovePlayerFromMatch = (byte)0x01,

    /// <summary>
    /// Event raised by individual client when a client avatar is spawned
    /// </summary>
    OnPlayerSpawnFinished = (byte)0x02,

    /// <summary>
    /// Event raised by individual client when the player pressed ready
    /// </summary>
    OnPlayerReady = (byte)0x03,

    /// <summary>
    /// Event raised by master client when all players are ready for actions
    /// </summary>
    OnPlayerSpawning = (byte)0x04,

    /// <summary>
    /// Event raised by master client when a round has finished
    /// </summary>
    OnRoundOver = (byte)0x05,

    /// <summary>
    /// Event raised by individual client when a client died
    /// </summary>
    OnPlayerDeath = (byte)0x12,
};
