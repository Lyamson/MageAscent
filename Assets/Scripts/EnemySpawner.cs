using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private Transform enemyPrefab;
    [SerializeField] float spawnEachSecond = 0.1f;
    [SerializeField] float timePassed = 0f;
    [SerializeField] int count = 200;

    private void Update()
    {
        if (!IsServer || count == 0) return;
        timePassed += Time.deltaTime;
        if (timePassed > spawnEachSecond)
        {
            count--;
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);
            Vector2 v = new Vector2(x * 30, y * 30);
            Transform spawnedEnemy = Instantiate(enemyPrefab, v, Quaternion.identity);
            spawnedEnemy.GetComponent<NetworkObject>().Spawn(true);
            timePassed = 0f;
        }
    }

    [ServerRpc]
    private void SpawnEnemyServerRpc()
    {
        Transform spawnedEnemy = Instantiate(enemyPrefab);
        spawnedEnemy.GetComponent<NetworkObject>().Spawn(true);
    }
}
