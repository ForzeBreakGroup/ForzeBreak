using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Jason Lin
 * 
 * Description:
 * PowerUp grade class for generating appropriate tier level power up
 */
public class PowerUpGrade
{
    /// <summary>
    /// Enum defines the tier level of power up
    /// </summary>
    public enum TierLevel
    {
        EPIC,
        RARE,
        COMMON,
    };

    /// <summary>
    /// Static name string for common tier level power up
    /// </summary>
    private static string[] commonPowerUp =
    {
        // StaticData.MISSILE_COMPONENT,
        StaticData.CANNON_COMPONENT,
        StaticData.SPIKERAM_COMPONENT,
        StaticData.HOTPOTATO_COMPONENT,
        StaticData.PUNCHINGGLOVE_COMPONENT,
        StaticData.MORNINGSTAR_COMPONENT,
        StaticData.SNOWBALL_COMPONENT,
		StaticData.BANNANTRAP_COMPONENT,
		//StaticData.SPRINGTRAP_COMPONENT,
    };

    /// <summary>
    /// Public method to randomly generate powerup name based on the tier level given
    /// </summary>
    /// <param name="tier"></param>
    /// <returns></returns>
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
