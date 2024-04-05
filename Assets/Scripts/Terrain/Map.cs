using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : NetworkBehaviour
{
    Dictionary<Vector2Int, GameObject> rooms = new();
    Dictionary<Vector2Int, GameObject> passages = new();
    public static Vector2Int[] faces = new Vector2Int[4] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
    [SerializeField] GameObject spawnRoomPrefab;
    [SerializeField] List<GameObject> roomPrefabs;
    [SerializeField] public List<GameObject> wallPrefabs;
    [SerializeField] public List<TileBase> floorPrefabs;

    [SerializeField] public Tilemap floorTilemap;

    public System.Random Random { get; private set; }

    public void SpawnPassage(Vector2Int position, Vector2Int face)
    {
        if (face != Vector2Int.up && face != Vector2Int.right)
        {
            Debug.LogError("Can't spawn a passage with face: " + face);
            return;
        }
        GameObject passage = new GameObject("Passage");
        passage.transform.parent = transform;
        Passage passageComponent = passage.AddComponent<Passage>();
        passageComponent.Spawn(position, face, this);
    }

    enum Side
    {
        LEFT,
        TOP,
        RIGHT,
        BOTTOM
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        DrawMapClientRpc((int) DateTime.Now.Subtract(DateTime.UnixEpoch).TotalSeconds);
    }

    [ClientRpc]
    public void DrawMapClientRpc(int seed)
    {
        Random = new System.Random(seed);
        var rooms = GenerateRooms(10, Random, 4);
        foreach (var room in rooms)
        {
            AddRoom(room.Key);
        }
        SpawnRooms();
    }

    private Dictionary<Vector2Int, ushort> GenerateRooms(ushort roomsCount, System.Random random, ushort maxDepth = 3)
    {
        Dictionary<Vector2Int, ushort> takenRoomSpots = new() { { Vector2Int.zero, 0 } };
        Dictionary<Vector2Int, ushort> availableRoomSpots = new();
        foreach (var face in faces)
        {
            availableRoomSpots.Add(face, 1);
        }

        for (int roomI = 0; roomI < roomsCount; roomI++)
        {
            KeyValuePair<Vector2Int, ushort> spot = Enumerable.ToList(availableRoomSpots)[random.Next(availableRoomSpots.Count)];
            takenRoomSpots.Add(spot.Key, spot.Value);
            availableRoomSpots.Remove(spot.Key);
            if (spot.Value == maxDepth) continue;
            foreach (var face in faces)
            {
                if (takenRoomSpots.ContainsKey(face + spot.Key) || availableRoomSpots.ContainsKey(face + spot.Key)) continue;
                availableRoomSpots.Add(face + spot.Key, (ushort)(spot.Value + 1));
            }
        }

        return takenRoomSpots;
    }

    public void AddRoom(Vector2Int position)
    {
        if (rooms.ContainsKey(position))
        {
            Debug.LogError("Room place already used: " + position);
        }
        else
        {
            GameObject room = new GameObject("Room");
            room.transform.parent = transform;
            room.transform.position = new Vector3(position.x * 25, position.y * 25);
            rooms.Add(position, room);
        }
    }

    public void SpawnRooms()
    {
        foreach (var room in rooms)
        {
            Room roomComponent = room.Value.AddComponent<Room>();
            if (room.Key == Vector2Int.zero)
            {
                roomComponent.Spawn(spawnRoomPrefab, room.Key, this);
                continue;
            }
            Debug.Log("Spawn room at " + room.Key);
            roomComponent.Spawn(roomPrefabs[Random.Next(roomPrefabs.Count)], room.Key, this);
            BoxCollider2D collider = room.Value.AddComponent<BoxCollider2D>();
            collider.offset = new Vector2(8f, 8f);
            collider.size = new Vector2(14f, 14f);
            collider.isTrigger = true;
        }
    }

    public bool HasRoom(Vector2Int position)
    {
        if (rooms.ContainsKey(position))
        {
            return true;
        }
        return false;
    }
}
