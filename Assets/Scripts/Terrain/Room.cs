using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    List<Vector2Int> enemySpawnPoints = new List<Vector2Int>();
    bool stepped = false;

    public void Spawn(GameObject roomPrefab, Vector2Int position, Map map)
    {
        List<Vector2Int> passages = new();

        foreach (var face in Map.faces)
        {
            if (map.HasRoom(position + face))
            {
                passages.AddRange(GetPassageWallsByFace(face));
                if (face == Vector2Int.up || face == Vector2Int.right)
                {
                    map.SpawnPassage(position, face);
                }
            }
        }

        foreach (Tilemap tilemap in roomPrefab.GetComponentsInChildren<Tilemap>())
        {
            if (tilemap.CompareTag("Floor"))
            {
                for (int x = 0; x <= tilemap.cellBounds.size.x; x++)
                {
                    for (int y = 0; y <= tilemap.cellBounds.size.y; y++)
                    {
                        if (tilemap.GetTile(new Vector3Int(x, y)) == null) continue;
                        map.floorTilemap.SetTile(new Vector3Int(x + position.x * 25, y + position.y * 25), tilemap.GetTile(new Vector3Int(x, y)));
                    }
                }
            }
            else if (tilemap.CompareTag("Wall"))
            {
                for (int x = 0; x <= tilemap.cellBounds.size.x; x++)
                {
                    for (int y = 0; y <= tilemap.cellBounds.size.y; y++)
                    {
                        //wallsTilemap.SetTile(new Vector3Int(x + offsetX, y + offsetY), tilemap.GetTile(new Vector3Int(x, y)));
                        if (tilemap.GetTile(new Vector3Int(x, y)) == null || passages.Contains(new Vector2Int(x, y))) continue;
                        Instantiate(map.wallPrefabs[map.Random.Next(map.wallPrefabs.Count)], new Vector2(x + position.x * 25, y + position.y * 25), Quaternion.identity, transform);
                        //walls.Add(new Vector2Int(x, y), wall);
                    }
                }
            }
            else if (tilemap.CompareTag("EnemySpawn"))
            {
                for (int x = 0; x <= tilemap.cellBounds.size.x; x++)
                {
                    for (int y = 0; y <= tilemap.cellBounds.size.y; y++)
                    {
                        //wallsTilemap.SetTile(new Vector3Int(x + offsetX, y + offsetY), tilemap.GetTile(new Vector3Int(x, y)));
                        if (tilemap.GetTile(new Vector3Int(x, y)) == null) continue;
                        enemySpawnPoints.Add(new Vector2Int(x + position.x * 25, y + position.y * 25));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets list of positions of walls that supposed to be opened for passage
    /// </summary>
    /// <param name="face">Direction of passage (Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left)</param>
    /// <returns></returns>
    private List<Vector2Int> GetPassageWallsByFace(Vector2Int face)
    {
        List<Vector2Int> passageWalls = new();
        if (face == Vector2Int.up)
        {
            for (ushort x = 6; x < 10; x++)
            {
                passageWalls.Add(new Vector2Int(x, 15));
            }
        }
        else if (face == Vector2Int.right)
        {
            for (ushort y = 6; y < 10; y++)
            {
                passageWalls.Add(new Vector2Int(15, y));
            }
        }
        else if (face == Vector2Int.down)
        {
            for (ushort x = 6; x < 10; x++)
            {
                passageWalls.Add(new Vector2Int(x, 0));
            }
        }
        else if (face == Vector2Int.left)
        {
            for (ushort y = 6; y < 10; y++)
            {
                passageWalls.Add(new Vector2Int(0, y));
            }
        }
        return passageWalls;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!NetworkManager.Singleton.IsServer || stepped || !collision.TryGetComponent(out PlayerMovement playerMovement)) return; // TODO: change getting component
        stepped = true;
        foreach (Vector2Int enemySpawnPoint in enemySpawnPoints)
        {
            GameObject spawnedEnemy = Instantiate(PrefabContainer.Singleton.GetPrefab(Enemy.EnemyType.SLIME), new Vector3(enemySpawnPoint.x, enemySpawnPoint.y), Quaternion.identity);
            Debug.Log("Spawn enemy at " + enemySpawnPoint);
            spawnedEnemy.GetComponent<NetworkObject>().Spawn(true);
        }
    }
}