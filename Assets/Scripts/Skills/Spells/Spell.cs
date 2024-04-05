using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : Skill
{
    public enum SpellType
    {
        NONE,
        BALL,
        FIREBALL,
        ICE
    }
    public abstract SpellType spellType { get; }
    public abstract string Name { get; }
    public abstract int price { get; }
}
