using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passage : MonoBehaviour
{
    public void Spawn(Vector2Int position, Vector2Int face, Map map)
    {
        if (face == Vector2Int.up)
        {
            for (int y = 16; y < 25; y++)
            {
                Instantiate(map.wallPrefabs[map.Random.Next(map.wallPrefabs.Count)], new Vector2(5 + position.x * 25, y + position.y * 25), Quaternion.identity, transform);
                Instantiate(map.wallPrefabs[map.Random.Next(map.wallPrefabs.Count)], new Vector2(10 + position.x * 25, y + position.y * 25), Quaternion.identity, transform);
                for (int x = 6; x < 10; x++)
                {
                    map.floorTilemap.SetTile(new Vector3Int(x + position.x * 25, y + position.y * 25), map.floorPrefabs[map.Random.Next(map.floorPrefabs.Count)]);
                }
            }
        }
        else if (face == Vector2Int.right)
        {
            for (int x = 16; x < 25; x++)
            {
                Instantiate(map.wallPrefabs[map.Random.Next(map.wallPrefabs.Count)], new Vector2(x + position.x * 25, 5 + position.y * 25), Quaternion.identity, transform);
                Instantiate(map.wallPrefabs[map.Random.Next(map.wallPrefabs.Count)], new Vector2(x + position.x * 25, 10 + position.y * 25), Quaternion.identity, transform);
                for (int y = 6; y < 10; y++)
                {
                    map.floorTilemap.SetTile(new Vector3Int(x + position.x * 25, y + position.y * 25), map.floorPrefabs[map.Random.Next(map.floorPrefabs.Count)]);
                }
            }
        }
    }
}
