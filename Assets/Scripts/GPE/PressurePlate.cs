using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider), typeof(NetworkObject))]
public class PressurePlate : NetworkBehaviour
{
    public event Action OnPlayerExit = null;
    [SerializeField] LayerMask playerMask = 1 << 6;
    [SerializeField] BoxCollider ownCollider = null;
    [SerializeField] bool drawGizmos = false;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform door;
    [SerializeField] float timeOpenDoor = 2;

    NetworkVariable<Vector3> syncPosition = new();
    NetworkVariable<Quaternion> syncRotation = new();
    private void Start()
    {
        Init();
    }
    private void Update()
    {
        if (IsClient)
        {
            door.position = syncPosition.Value;
            door.rotation = syncRotation.Value;
        }
        else
        {
            syncPosition.Value = door.position;
            syncRotation.Value = door.rotation;
        }
    }
    private void Init()
    {
        rb.isKinematic = true;
        OnPlayerExit += () =>
        {
            rb.isKinematic = false;
            Invoke(nameof(OpenDoor), timeOpenDoor);
        };
    }
    private void OnTriggerExit(Collider _collier)
    {
        if (IsClient || (1 << _collier.gameObject.layer) != playerMask)
            return;
        OnPlayerExit?.Invoke();
    }
    void OpenDoor()
    {
        rb.isKinematic = true;
        door.position = transform.position;
        door.rotation = transform.rotation;
    }
    private void OnDrawGizmos()
    {
        if (!ownCollider || !rb || !drawGizmos)
            return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(ownCollider.bounds.center, ownCollider.bounds.size);
    }
}
