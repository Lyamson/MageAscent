using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballProjectile : Projectile
{
    [NonSerialized] public float damage;

    public override void OnTargetHit(Unit target)
    {
        target.DealDamage(damage);
    }
}
