using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TombstoneBehaviour : MonoBehaviour
{
    Player player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Tombstone entered");
    }
}
