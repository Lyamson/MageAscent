using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SkillsManager : NetworkBehaviour
{
    KeyCode[] keyCodes;
    List<Spell> learntSpells = new();
    Dictionary<KeyCode, Spell> quickSpells = new Dictionary<KeyCode, Spell>();

    uint skillPoints = 0;
    public uint SkillPoints { get => skillPoints; }

    private void Awake()
    {
        keyCodes = new KeyCode[4];
        for (int i = 0; i < 4; i++)
        {
            keyCodes[i] = (KeyCode)(49 + i);
        }
    }

    [NonSerialized] public Spell activeSpell;

    public void ActivateSpell(KeyCode key)
    {
        MyDebug.Log(this, "Activate spell for " + key);
        GameManager.Singleton.SetActiveSlot((byte)(key - 49 + 1));
        if (quickSpells.TryGetValue(key, out Spell spell))
        {
            activeSpell = spell;
        }
        else
        {
            activeSpell = null;
        }
    }

    public bool HasSpell(Spell.SpellType itemSpellType)
    {
        Debug.Log("SpellType Comparing: " + itemSpellType);
        foreach (Spell spell in learntSpells)
        {
            if (spell.spellType == itemSpellType)
            {
                Debug.Log("SpellType Comparing: true");
                return true;
            }
        }
        return false;
    }

    void SetQuickSpell(Spell spell, KeyCode keyCode)
    {
        byte slot = (byte)(keyCode - 49 + 1);
        if (quickSpells.TryGetValue(keyCode, out Spell currentSpell))
        {
            currentSpell.slot = 8;
        }
        quickSpells.Add(keyCode, spell);
        spell.slot = slot;
        GameManager.Singleton.SetQuickSpell(spell.spellType, slot);
    }

    internal void LearnSpell(Spell.SpellType itemSpellType)
    {
        switch (itemSpellType)
        {
            case Spell.SpellType.NONE:
                break;
            case Spell.SpellType.BALL:
                LearnSpell<BallSpell>();
                break;
            case Spell.SpellType.FIREBALL:
                LearnSpell<FireballSpell>();
                break;
            case Spell.SpellType.ICE:
                LearnSpell<IceSpell>();
                break;
            default:
                break;
        }
    }

    public void LearnSpell<T>() where T : Spell, new()
    {
        MyDebug.Log(this, "LearnSpell Callback");
        Spell spell = gameObject.AddComponent<T>();
        spell.skillsManager = this;
        learntSpells.Add(spell);
        if (quickSpells.Count < 4)
        {
            foreach (KeyCode keyCode in keyCodes)
            {
                if (quickSpells.ContainsKey(keyCode)) continue;
                SetQuickSpell(spell, keyCode);
                break;
            }
        }
    }
    #region BallSpell
    [ServerRpc]
    public void CastBallSpellServerRpc(Vector2 position, Vector2 direction, float damage, float projectileForce, TeamLogic.Team team)
    {
        MyDebug.Log(this, "CastBallSpellServerRpc Callback");
        CastBallSpellClientRpc(position, direction, damage, projectileForce, team);
    }

    [ClientRpc]
    public void CastBallSpellClientRpc(Vector2 position, Vector2 direction, float damage, float projectileForce, TeamLogic.Team team)
    {
        MyDebug.Log(this, "CastBallSpellClientRpc Callback");
        BallSpell.CastLocally(position, direction, damage, projectileForce, team);
    }
    #endregion

    #region MeleeAttackSkill
    [ServerRpc]
    public void CastMeleeAttackSkillServerRpc(float damage, ulong targetNetworkObjectId)
    {
        MyDebug.Log(this, "CastMeleeAttackSkillServerRpc Callback");
        CastMeleeAttackSkillClientRpc(damage, targetNetworkObjectId);
    }

    [ClientRpc]
    public void CastMeleeAttackSkillClientRpc(float damage, ulong targetNetworkObjectId)
    {
        MyDebug.Log(this, "CastMeleeAttackSkillClientRpc Callback");
        NetworkObject targetObject = GetNetworkObject(targetNetworkObjectId);
        if (targetObject?.gameObject != null)
        {
            MeleeAttackSkill.CastLocally(damage, targetObject.gameObject);
        }
    }
    #endregion

    #region FireballSpell
    [ServerRpc]
    public void CastFireballSpellServerRpc(Vector2 startLocation, Vector2 direction, float totalDamage, float projectileForce, TeamLogic.Team team)
    {
        MyDebug.Log(this, "CastFireballSpellServerRpc Callback");
        CastFireballSpellClientRpc(startLocation, direction, totalDamage, projectileForce, team);
    }

    [ClientRpc]
    public void CastFireballSpellClientRpc(Vector2 startLocation, Vector2 direction, float totalDamage, float projectileForce, TeamLogic.Team team)
    {
        MyDebug.Log(this, "CastFireballSpellClientRpc Callback");
        FireballSpell.CastLocally(startLocation, direction, totalDamage, projectileForce, team);
    }
    #endregion

    #region IceSpell
    [ServerRpc]
    public void CastIceSpellServerRpc(Vector2 startLocation, Vector2 direction, float totalDamage, float projectileForce, TeamLogic.Team team)
    {
        MyDebug.Log(this, "CastFireballSpellServerRpc Callback");
        CastIceSpellClientRpc(startLocation, direction, totalDamage, projectileForce, team);
    }

    [ClientRpc]
    public void CastIceSpellClientRpc(Vector2 startLocation, Vector2 direction, float totalDamage, float projectileForce, TeamLogic.Team team)
    {
        MyDebug.Log(this, "CastFireballSpellClientRpc Callback");
        IceSpell.CastLocally(startLocation, direction, totalDamage, projectileForce, team);
    }
    #endregion

    public void AddSkillPoint()
    {
        skillPoints += 1;
        if (skillPoints >= 1)
        {
            GameManager.Singleton.ShowSkillPointButton();
        }
        else
        {
            GameManager.Singleton.HideSkillPointButton();
        }
    }

    public bool RemoveSkillPoints(uint pointsCost)
    {
        if (skillPoints >= pointsCost)
        {
            skillPoints = (uint)(skillPoints - pointsCost);
            if (skillPoints == 0)
            {
                GameManager.Singleton.HideSkillPointButton();
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Fully resets SkillManager.
    /// </summary>
    public void Reset()
    {
        RemoveSkillPoints(skillPoints);
    }
}
