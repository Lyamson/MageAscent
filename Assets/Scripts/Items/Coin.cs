using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Item
{
    [SerializeField] int _value = 10;
    public override int value => _value;

    public override ItemType type => ItemType.COIN;
}
