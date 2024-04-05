using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MeleeAttackSkill : Skill
{
    protected override float cooldown => 4f;
    [SerializeField] float damage = 40f;

    public override bool AutoUseLogic()
    {
        MyDebug.Log(this, "MeleeAttackSkill.AutoUse Callback");
        bool used = false;
        try
        {
            Collider2D[] targets = Physics2D.OverlapCircleAll(owner.GetPosition(), autoCastRadius, owner.GetEnemyHitBoxLayerMask());
            foreach (Collider2D collider in targets)
            {
                Unit unit = collider.gameObject.GetComponentInParent<Unit>();
                if (unit == null || !TeamLogic.IsEnemies(unit, owner)) continue;
                used = true;
                skillsManager?.CastMeleeAttackSkillServerRpc(damage, unit.NetworkObjectId);
            }
        }
        catch (Exception ex)
        {
            MyDebug.Log(this, "Can't AutoUse: " + ex.Message);
        }

        return used;
    }

    public override bool UseLogic()
    {
        throw new System.NotImplementedException();
    }

    public static void CastLocally(float damage, GameObject targetObject)
    {
        Unit unit = targetObject.GetComponent<Unit>();
        if (unit != null)
        {
            unit.DealDamage(damage);
        }
    }
}
