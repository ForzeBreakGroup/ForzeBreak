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
    /// Event raised by individual client, when a client died
    /// </summary>
    OnPlayerDeath = (byte)0x12,
};
