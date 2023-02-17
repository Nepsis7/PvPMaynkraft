using Unity.Netcode;
using UnityEngine;

public partial class Player : NetworkBehaviour
{
    [ServerRpc]
    void RefreshPositionServerRpc(Vector3 _position)
    {
        transform.position = _position;
        syncedPosition.Value = _position;
    }
    [ServerRpc]
    void RefreshRotationServerRpc(float _yaw)
    {
        transform.eulerAngles = Vector3.up * _yaw;
        syncedYaw.Value = _yaw;
    }
    [ServerRpc] void AddFallServerRpc() => DeathBoard.Instance?.AddFallServer(PlayerID);
    [ServerRpc] void HitPlayerServerRpc(ulong _playerID, Vector3 _dirNormalized) => PlayerManager.Instance?[_playerID].KnockBackIfLocalPlayerClientRpc(_dirNormalized);
}
