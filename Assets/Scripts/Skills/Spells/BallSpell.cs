using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BallSpell : Spell
{
    float damage = 1f;
    float projectileForce = 8f;

    float damageAdding = 1f;

    public override SpellType spellType => SpellType.BALL;

    public override string Name => "Ball";

    public override int price => 0;

    protected override void Awake()
    {
        base.Awake();
    }

    public override bool UseLogic()
    {
        MyDebug.Log(this, "BallSpell.Cast Callback 1");
        Player player = owner as Player;
        if (player == null) return false;
        MyDebug.Log(this, "BallSpell.Cast Callback 2");
        Camera camera = player.playerCamera;
        Vector2 mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 myPos = player.GetPosition();
        Vector2 direction = (mousePos - myPos).normalized;
        float totalDamage = damage + damageAdding;
        skillsManager.CastBallSpellServerRpc(myPos, direction, totalDamage, projectileForce, owner.team);

        return true;
    }

    public override bool AutoUseLogic()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(owner.GetPosition(), autoCastRadius, owner.GetEnemyHitBoxLayerMask());
        bool used = false;
        foreach (Collider2D collider in targets)
        {
            Unit unit = collider.gameObject.GetComponentInParent<Unit>();
            if (unit == null || !TeamLogic.IsEnemies(unit, owner)) continue;
            used = true;
            Vector2 myPos = owner.GetPosition();
            Vector2 direction = (unit.GetPosition() - myPos).normalized;
            float totalDamage = damage + damageAdding;
            skillsManager.CastBallSpellServerRpc(myPos, direction, totalDamage, projectileForce, owner.team);
        }
        return used;
    }

    public override void SpellUsed()
    {
        base.SpellUsed();
        damageAdding++;
    }

    public static void CastLocally(Vector2 position, Vector2 direction, float damage, float projectileForce, TeamLogic.Team team)
    {
        print("CastInstance Callback");
        GameObject spell = Instantiate(PrefabContainer.Singleton.GetPrefab(SpellType.BALL, TeamLogic.IsFriendly(team)), position, Quaternion.identity);

        spell.GetComponent<TestProjectile>().damage = damage;
        spell.GetComponent<Rigidbody2D>().velocity = direction * projectileForce;
    }
}
