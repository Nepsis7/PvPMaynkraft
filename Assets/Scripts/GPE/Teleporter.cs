using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Teleporter : MonoBehaviour
{
    public event Action OnPlayerEnter = null;
    [SerializeField] Transform teleportTo = null;
    [SerializeField] LayerMask playerMask = 1 << 6;
    [SerializeField] bool drawGizmos = false;
    private void OnTriggerEnter(Collider _collider)
    {
        if ((1 << _collider.gameObject.layer) != playerMask)
            return;
        Player _player = _collider.GetComponent<Player>();
        if (!_player && !teleportTo)
            return;
        _player.SetPositionIfLocalPlayer(teleportTo.position);
        OnPlayerEnter?.Invoke();
    }

    private void OnDrawGizmos()
    {
        if (!teleportTo || !drawGizmos)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
        Gizmos.DrawLine(transform.position, teleportTo.position);
        Gizmos.DrawWireSphere(teleportTo.position, 0.1f);
    }
}
