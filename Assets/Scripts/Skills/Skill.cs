using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Skill : NetworkBehaviour
{
    [SerializeField] protected virtual float cooldown => 3f;
    [SerializeField] protected float autoCastRadius = 3f;
    public SkillsManager skillsManager;
    public byte slot = 8;

    protected float currentCooldown = 0f;

    /// <summary>
    /// «начение перезар€дки при запуске перезар€дки
    /// </summary>
    protected float _cooldownFromStart;

    protected Unit owner;

    protected virtual void Awake()
    {
        print("Skill.Start Callback");
        owner = gameObject.GetComponentInParent<Unit>();
    }

    private void Update()
    {
        if (currentCooldown > 0) DecreaseCooldown(Time.deltaTime);
    }

    private void DecreaseCooldown(float passedTime)
    {
        currentCooldown = MathF.Max(0f, currentCooldown - passedTime);
        if (slot < 5) GameManager.Singleton.SetCooldown(slot, currentCooldown / _cooldownFromStart);
    }

    public void Use()
    {
        if (IsReadyToCast() && UseLogic())
        {
            SpellUsed();
        }
    }

    public void AutoUse()
    {
        if (AutoUseLogic())
        {
            SpellUsed();
        }
    }

    /// <summary>
    /// Call ServerRpc here
    /// </summary>
    /// <returns>Whether used successfully or not</returns>
    public abstract bool UseLogic();

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Whether used successfully or not</returns>
    public abstract bool AutoUseLogic();

    public virtual void SpellUsed()
    {
        StartCooldown();
    }

    public float GetCurrentCooldown()
    {
        return currentCooldown;
    }

    public virtual bool IsReadyToCast()
    {
        return currentCooldown == 0f;
    }

    public virtual bool IsReadyToAutoCast()
    {
        return currentCooldown == 0f && Physics2D.OverlapCircle(owner.GetPosition(), autoCastRadius, owner.GetEnemyHitBoxLayerMask()) != null;
    }

    protected void StartCooldown()
    {
        _cooldownFromStart = MathF.Max(0f, cooldown * (1f - owner.GetCooldownReduction() / 100f));
        currentCooldown = _cooldownFromStart;
    }
}