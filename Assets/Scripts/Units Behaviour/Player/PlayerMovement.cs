using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : NetworkBehaviour
{
    //[SerializeField] private GameObject coinPrefab;
    [SerializeField] Stats stats;

    Vector2 _faceDirection;
    Vector2 _lastDirection;

    Animator _animator;
    Rigidbody2D _body;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!IsOwner) return;
        TakeInput();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        Move();
    }

    private void Move()
    {
        //transform.Translate(_faceDirection.normalized * speed * Time.deltaTime);
        _body.velocity = _faceDirection.normalized * stats.Speed;

        if(_faceDirection == Vector2.zero)
        {
            _animator.SetLayerWeight(1, 0);
        }
        else
        {
            _animator.SetLayerWeight(1, 1);
            SetAnimatorMovement(_lastDirection);
        }
    }

    private void TakeInput()
    {
        _faceDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (_faceDirection != Vector2.zero)
            _lastDirection = _faceDirection;
    }

    private void SetAnimatorMovement(Vector2 direction)
    {
        _animator.SetFloat("xDir", direction.x);
        _animator.SetFloat("yDir", direction.y);
    }
}
