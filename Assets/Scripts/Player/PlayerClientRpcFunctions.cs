using Unity.Netcode;
using UnityEngine;

public partial class Player : NetworkBehaviour
{
    [ClientRpc]
    void KnockBackIfLocalPlayerClientRpc(Vector3 _dirNormalized)
    {
        if (IsLocalPlayer && canGetHit)
        {
            rb.AddForce(4 * new Vector3(_dirNormalized.x * kbMultipliers.Horizontal, _dirNormalized.y * kbMultipliers.Vertical, _dirNormalized.z * kbMultipliers.Horizontal), ForceMode.Impulse); ;
            canGetHit = false;
            Invoke("AllowHits", hitDelay);
        }
    }
    void AllowHits() => canGetHit = true;
}
