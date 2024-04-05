using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAttack : NetworkBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject playerCharacter;
    [SerializeField] private float damage;
    [SerializeField] private float projectileForce = 2f;

    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetMouseButtonDown(1))
        {
            MyDebug.Log(this, "mouse pressed");
            Camera camera = gameObject.GetComponentInParent<Player>().playerCamera;
            Vector2 mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 myPos = playerCharacter.transform.position;
            Vector2 direction = (mousePos - myPos).normalized;
            TestSpellServerRpc(new Vector3(myPos.x, myPos.y), direction);
        }
    }

    [ServerRpc]
    private void TestSpellServerRpc(Vector3 position, Vector2 direction)
    {
        MyDebug.Log(this, "TestSpellServerRpc Callback");
        TestSpellClientRpc(position, direction);
    }

    [ClientRpc]
    private void TestSpellClientRpc(Vector3 position, Vector2 direction)
    {
        GameObject spell = Instantiate(projectilePrefab, position, Quaternion.identity);
        spell.GetComponent<TestProjectile>().damage = damage;
        spell.GetComponent<Rigidbody2D>().velocity = direction * projectileForce;
    }
}