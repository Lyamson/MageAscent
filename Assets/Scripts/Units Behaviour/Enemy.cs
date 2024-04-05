using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Unit
{
    public enum EnemyType
    {
        SLIME
    }

    [SerializeField] private List<GameObject> lootDrops;

    private void Start()
    {
        MyDebug.Log(this, "Enemy.Start Callback");
        team = TeamLogic.Team.ENEMY;
        Death += OnEnemyDeath;
    }

    private void OnEnemyDeath(object sender, EventArgs e)
    {
        if (!IsServer) return;
        Vector2 center = this.GetPosition();
        SpawnLootClientRpc(center);
        gameObject.GetComponent<NetworkObject>().Despawn();
    }

    [ClientRpc]
    protected void SpawnLootClientRpc(Vector2 center)
    {
        foreach (GameObject drop in lootDrops)
        {
            GameObject fieldDrop = Instantiate(drop, center, Quaternion.identity);
            fieldDrop.transform.Translate(new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)));
        }
    }

    public override int GetEnemyHitBoxLayerMask()
    {
        return 1 << 3;
    }
}
