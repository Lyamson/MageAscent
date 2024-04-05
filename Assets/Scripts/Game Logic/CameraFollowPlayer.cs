using Unity.Netcode;
using UnityEngine;

public class CameraFollowPlayer : NetworkBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothing = .05f;
    private Vector3 offset = new Vector3(0, 0, -10);

    private void FixedUpdate()
    {
        if (player == null) return;
        Vector3 newPos = Vector3.Lerp(transform.position, player.position + offset, smoothing);
        transform.position = newPos;
    }
}
