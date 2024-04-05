using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exp : Item
{
    [SerializeField] int _value = 2;
    public override int value => _value;

    public override ItemType type => ItemType.EXP;
}
