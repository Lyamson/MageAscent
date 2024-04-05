using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HitBox"))
        {
            Unit unit = collision.GetComponentInParent<Unit>();
            OnTargetHit(unit);
        }
        Destroy(gameObject);
    }

    public abstract void OnTargetHit(Unit target);
}
