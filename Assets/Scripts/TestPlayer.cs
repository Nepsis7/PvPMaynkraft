using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class TestPlayer : NetworkBehaviour
{
    void Update()
    {
        if (IsOwner)
            Move();
    }
    void Move()
    {
        transform.position += Time.deltaTime * 2 * (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal"));
        RefreshMovementServerRpc(transform.position);
    }
    [ServerRpc]
    void RefreshMovementServerRpc(Vector3 _position)
    {
        transform.position = _position;
        RefreshMovementClientRpc(_position);
    }
    [ClientRpc]
    void RefreshMovementClientRpc(Vector3 _position)
    {
        if (IsOwner)
            return;
        transform.position = _position;
    }
}
