using Unity.Netcode;
using UnityEngine;

public class TestProjectile : MonoBehaviour
{
    public float damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HitBox"))
        {
            Unit unit = collision.GetComponentInParent<Unit>();
            //if (!TeamLogic.IsEnemies(team, unit)) return;
            unit.DealDamage(damage);
        }
        Destroy(gameObject);
    }
}
