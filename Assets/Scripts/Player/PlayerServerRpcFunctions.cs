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
    [ServerRpc] void AddFallServerRpc() => DeathBoard.Instance?.AddFallServer(NetworkObjectId);
    [ServerRpc] void HitPlayerServerRpc(ulong _playerID, Vector3 _dirNormalized) => NetworkManager.SpawnManager.SpawnedObjects[_playerID].GetComponent<Player>().KnockBackIfLocalPlayerClientRpc(_dirNormalized);
}
