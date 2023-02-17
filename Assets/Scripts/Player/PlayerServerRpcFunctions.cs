using System.Collections.Generic;
using Unity.Collections;
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
    [ServerRpc]
    void SetUsernameServerRpc(string _username)
    {
        List<FixedString32Bytes> _takenUsernames = new();
        foreach (NetworkObject _spawnedObject in NetworkManager.SpawnManager.SpawnedObjects.Values)
        {
            Player _player = _spawnedObject.GetComponent<Player>();
            if (!_player)
                continue;
            _takenUsernames.Add(_player.Username);
        }
        string _newUsername = _username;
        uint i = 1;
        while (_takenUsernames.Contains(_newUsername))
        {
            i++;
            _newUsername = _username + i;
        }
        Username = _newUsername;
        OnUsernameChanged?.Invoke(_newUsername);
    }
    [ServerRpc] void AddFallServerRpc() => DeathBoard.Instance?.AddFallServer(PlayerID);
    [ServerRpc] void HitPlayerServerRpc(ulong _playerID, Vector3 _dirNormalized) => PlayerManager.Instance?[_playerID].KnockBackIfLocalPlayerClientRpc(_dirNormalized);
}
