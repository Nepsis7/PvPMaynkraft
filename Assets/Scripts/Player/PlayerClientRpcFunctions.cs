using Unity.Netcode;
using UnityEngine;

public partial class Player : NetworkBehaviour
{
    [ClientRpc]
    void KnockBackIfLocalPlayerClientRpc(Vector3 _dirNormalized)
    {
        if (IsLocalPlayer && canGetHit)
        {
            rb.AddForce(4 * new Vector3(_dirNormalized.x * movementSettings.KbMultipliers.Horizontal, _dirNormalized.y * movementSettings.KbMultipliers.Vertical, _dirNormalized.z * movementSettings.KbMultipliers.Horizontal), ForceMode.Impulse);
            canGetHit = false;
            Invoke(nameof(AllowHits), movementSettings.HitDelay);
        }
    }
    void AllowHits() => canGetHit = true;
}
