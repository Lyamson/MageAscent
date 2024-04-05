using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public enum ItemType
    {
        COIN,
        RUBY,
        EXP
    };

    public abstract int value { get; }
    public abstract ItemType type { get; }
}
