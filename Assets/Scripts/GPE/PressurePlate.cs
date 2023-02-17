using System;
using Unity.Netcode;
using UnityEngine;

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
    NetworkVariable<Vector3> syncVelocity = new();
    private void Start()
    {
        Init();
    }
    private void Update()
    {
        if (rb.isKinematic)
            return;
        if (IsClient)
        {
            door.position = syncPosition.Value;
            door.rotation = syncRotation.Value;
            rb.velocity = syncVelocity.Value;
        }
        else
        {
            syncPosition.Value = door.position;
            syncRotation.Value = door.rotation;
            syncVelocity.Value = rb.velocity;
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
    private void OnTriggerExit(Collider _collider)
    {
        if ((1 << _collider.gameObject.layer) != playerMask)
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
