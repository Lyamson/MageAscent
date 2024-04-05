using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FloatToPlayer : MonoBehaviour
{
    [SerializeField] private float speed;
    public GameObject target;

    private void Update()
    {
        if (target == null) return;
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    }
}