using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemPickup : NetworkBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsOwner || !other.CompareTag("PickableItem") || SceneManager.GetActiveScene().name == "Lobby") return;
        Item item = other.gameObject.GetComponentInChildren<Item>();
        switch (item.type)
        {
            case Item.ItemType.COIN:
                playerStats.AddCoins(item.value);
                break;
            case Item.ItemType.EXP:
                playerStats.AddExp(item.value);
                break;
            default:
                break;
        }
        Destroy(other.gameObject);
    }
}
