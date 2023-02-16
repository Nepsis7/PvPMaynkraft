using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BouncePad : MonoBehaviour
{
    public event Action OnPlayerEnter = null;
    [SerializeField] LayerMask playerMask = 1 << 6;
    [SerializeField] BouncePadSettings settings = null;
    [SerializeField] bool drawGizmos = false;
    private void OnTriggerEnter(Collider _collider)
    {
        if ((1 << _collider.gameObject.layer) != playerMask)
            return;
        Player _player = _collider.GetComponent<Player>();
        if (!_player && !settings)
            return;
        print("test");
        _player.AddForceIfLocalPlayer(settings.Direction);
        OnPlayerEnter?.Invoke();
    }
    private void OnDrawGizmos()
    {
        if (!settings || !drawGizmos)
            return;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + settings.Direction);
        Gizmos.DrawWireSphere(transform.position + settings.Direction, .1f);
    }
}
