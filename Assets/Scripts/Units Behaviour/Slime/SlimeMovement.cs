using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SlimeMovement : NetworkBehaviour
{
    [SerializeField] float speed = 2.5f;
    [SerializeField] Transform SpriteControllerTransform;
    [SerializeField] Transform CharacterTransform;
    [SerializeField] Rigidbody2D _body;

    Vector2 _movementDirection;

    NetworkVariable<ulong> targetTransform = new();

    private void Start()
    {
        if (IsServer) SetTarget();
    }

    private void Update()
    {
        Player player;
        if (GameManager.Singleton == null || !GameManager.Singleton.players.TryGetValue(targetTransform.Value, out player)) return;
        if (!player.IsAlive)
        {
            if (IsServer) SetTarget();
            return;
        }

        if (Vector2.Distance(player.GetPosition(), CharacterTransform.position) <= .65f) return;

        _movementDirection = (player.GetPosition() - (Vector2)CharacterTransform.position).normalized;
        if (_movementDirection.x >= 0f)
        {
            SpriteControllerTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            SpriteControllerTransform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    private void FixedUpdate()
    {
        Move(_movementDirection);
        _movementDirection = Vector2.zero;
    }

    /// <summary>
    /// Sets random target
    /// </summary>
    void SetTarget()
    {
        if (TryChooseTarget(out ulong targetId))
        {
            targetTransform.Value = targetId;
        }
    }

    void SetTarget(ulong targetId)
    {
        targetTransform.Value = targetId;
    }

    bool TryChooseTarget(out ulong targetId)
    {
        List<ulong> targets = new();
        foreach (KeyValuePair<ulong, NetworkClient> pair in NetworkManager.Singleton.ConnectedClients)
        {
            if (pair.Value.PlayerObject.gameObject.GetComponent<Player>().IsAlive)
            {
                targets.Add(pair.Key);
            }
        }

        if (targets.Count > 0)
        {
            targetId = targets[Random.Range(0, targets.Count)];
            return true;
        }
        targetId = 0;
        return false;
    }

    private void Move(Vector2 direction)
    {
        //CharacterTransform.Translate(direction * speed * Time.deltaTime);
        _body.velocity = direction * speed;
    }
}
