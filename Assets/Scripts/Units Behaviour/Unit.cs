using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Unit : NetworkBehaviour
{
    [SerializeField] protected Stats stats;

    [SerializeField] protected GameObject character;
    [SerializeField] protected GameObject body;
    public bool IsAlive = true;
    public event EventHandler Death;
    public TeamLogic.Team team;

    public void DealDamage(float damage)
    {
        if (!IsOwner) return;
        MyDebug.Log(this, "Dealing damage " + damage);
        if (damage < stats.health.Value)
        {
            stats.health.Value -= damage;
        }
        else
        {
            stats.health.Value = 0;
        }
    }

    public void Die()
    {
        MyDebug.Log(this, "Dying");
        IsAlive = false;
        OnDeath(this, new());
    }

    public void Heal(float amount)
    {
        MyDebug.Log(this, $"Healing {amount} health");
        stats.health.Value = Math.Min(stats.maxHealth.Value, stats.health.Value + amount);
    }

    protected virtual void OnDeath(object sender, EventArgs e)
    {
        MyDebug.Log(this, "OnDeath Callback");
        Death?.Invoke(sender, e);
    }

    public Vector2 GetPosition()
    {
        return body.transform.position;
    }

    [ClientRpc]
    public void SetPositionClientRpc(Vector2 newPosition)
    {
        character.transform.position = newPosition;
    }

    public abstract int GetEnemyHitBoxLayerMask();

    public int GetCooldownReduction()
    {
        return stats.AllCooldownReduction;
    }
}