using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballSpell : Spell
{
    protected override float cooldown => .3f;
    float damage = 20f;
    float projectileForce = 8f;

    public override SpellType spellType => SpellType.FIREBALL;

    public override string Name => "Fire ball";

    public override int price => 100;

    public override bool AutoUseLogic()
    {
        throw new System.NotImplementedException();
    }

    public override bool UseLogic()
    {
        Player player = owner as Player;
        if (player == null) return false;
        Camera camera = player.playerCamera;
        Vector2 mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPos = player.GetPosition();
        Vector2 direction = (mousePos - playerPos).normalized;
        Vector2 startLocation = new Vector2(playerPos.x, playerPos.y - 0.2f);

        float totalDamage = damage + player.GetAttackDamage();

        skillsManager.CastFireballSpellServerRpc(startLocation, direction, totalDamage, projectileForce, owner.team);

        return true;
    }

    public static void CastLocally(Vector2 startLocation, Vector2 direction, float totalDamage, float projectileForce, TeamLogic.Team team)
    {
        print("FireballSpell.CastLocally Callback");
        GameObject fireball = Instantiate(PrefabContainer.Singleton.GetPrefab(SpellType.FIREBALL, TeamLogic.IsFriendly(team)), startLocation, Quaternion.identity);

        fireball.transform.right = direction;
        fireball.GetComponent<FireballProjectile>().damage = totalDamage;
        fireball.GetComponent<Rigidbody2D>().velocity = direction * projectileForce;
        Destroy(fireball, 5f);
    }
}
