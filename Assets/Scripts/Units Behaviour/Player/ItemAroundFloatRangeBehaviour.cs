using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemAroundFloatRangeBehaviour : NetworkBehaviour
{
    [SerializeField] private GameObject characterSpriteObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsOwner || !collision.CompareTag("PickableItem")) return;
        if (collision.TryGetComponent(out FloatToPlayer floatToPlayerBehaviour))
        {
            floatToPlayerBehaviour.target = characterSpriteObject;
        }
    }
}
