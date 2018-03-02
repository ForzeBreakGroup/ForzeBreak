using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpGrade
{
    public enum TierLevel
    {
        EPIC,
        RARE,
        COMMON,
    };

    private static string[] commonPowerUp =
    {
        StaticData.MISSILE_NAME,
        StaticData.CANNON_NAME,
        StaticData.SPIKERAM_NAME
    };

    public string GetRandomPowerUp(TierLevel tier)
    {
        string[] poolBase;

        switch (tier)
        {
            case TierLevel.EPIC:
            case TierLevel.RARE:
            case TierLevel.COMMON:
            default:
                poolBase = commonPowerUp;
                break;
        }

        // Randomly generate a number in between 0 and 10000
        // Mod the number by number of available powerups in the pool
        int index = Random.Range(0, 10000) % poolBase.Length;

        // Return the powerup name
        return poolBase[index];
    }
}
