using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamLogic : MonoBehaviour
{
    public enum Team
    {
        PLAYER,
        ENEMY
    }

    public static bool IsEnemies(Unit unit1, Unit unit2)
    {
        if (((unit1 is Enemy && unit2 is Player) || (unit1 is Player && unit2 is Enemy)) && !unit1.Equals(unit2)) return true;
        return false;
    }

    public static bool IsEnemies(Team unit1, Unit unit2)
    {
        if ((unit1 == Team.ENEMY && unit2 is Player) || (unit1 == Team.PLAYER && unit2 is Enemy)) return true;
        return false;
    }

    public static bool IsFriendly(Team team)
    {
        return team == Team.PLAYER;
    }
}
