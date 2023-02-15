using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(DeathBoard))]
public class Player : NetworkBehaviour
{
    [SerializeField] float deathHeight = -1f;
    Rigidbody rb = null;
    DeathBoard deathBoard = null;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        deathBoard = GetComponent<DeathBoard>();
        Respawn();
    }
    void Update()
    {
        if (IsOwner)
            Move();
        CheckDeathClientRpc();
    }
    void Move()
    {
        rb.position += Time.deltaTime * 2 * (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal"));
        RefreshMovementServerRpc(transform.position);
    }
    [ClientRpc]
    void CheckDeathClientRpc()
    {
        if (IsOwner && transform.position.y <= deathHeight)
        {
            Respawn();
            deathBoard.AddFall(NetworkObjectId);
        }

    }
    void Respawn()
    {
        if (SpawnManager.Instance)
            transform.position = SpawnManager.Instance.GetSpawnPoint();
    }
    [ServerRpc]
    void RefreshMovementServerRpc(Vector3 _position)
    {
        rb.position = _position;
        RefreshMovementClientRpc(_position);
    }
    [ClientRpc]
    void RefreshMovementClientRpc(Vector3 _position)
    {
        if (IsOwner)
            return;
        rb.position = _position;
    }
}
