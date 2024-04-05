using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSpell : Spell
{
    public override SpellType spellType => SpellType.ICE;

    protected override float cooldown => .8f;

    public override string Name => "Ice shard";

    public override int price => 200;

    float damage = 40f;
    float projectileForce = 10f;

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

        skillsManager.CastIceSpellServerRpc(startLocation, direction, totalDamage, projectileForce, owner.team);

        return true;
    }

    public static void CastLocally(Vector2 startLocation, Vector2 direction, float totalDamage, float projectileForce, TeamLogic.Team team)
    {
        print("IceSpell.CastLocally Callback");
        GameObject ice = Instantiate(PrefabContainer.Singleton.GetPrefab(SpellType.ICE, TeamLogic.IsFriendly(team)), startLocation, Quaternion.identity);

        ice.transform.right = direction;
        ice.GetComponent<IceProjectile>().damage = totalDamage;
        ice.GetComponent<Rigidbody2D>().velocity = direction * projectileForce;
        Destroy(ice, 5f);
    }
}
