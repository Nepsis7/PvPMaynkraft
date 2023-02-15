using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Globalization;

public class DeathBoard : NetworkBehaviour
{
    Dictionary<ulong, int> deaths = new Dictionary<ulong, int>();
    public void AddFall(ulong _playerID) { if (IsOwner) AddFallServerRpc(_playerID); }
    [ServerRpc]
    void AddFallServerRpc(ulong _playerID)
    {
        //transform.position = Vector3.one * 3;
        AddFall_Internal(_playerID);
        RefreshDeathsClientRpc(_playerID);
    }
    [ClientRpc]
    void RefreshDeathsClientRpc(ulong _playerID)
    {
        AddFall_Internal(_playerID);
    }
    void AddFall_Internal(ulong _playerID)
    {
        if (!deaths.ContainsKey(_playerID))
            deaths.Add(_playerID, 0);
        deaths[_playerID]++;
    }
    private void OnGUI()
    {
        if (IsOwner)
            foreach (KeyValuePair<ulong, int> _deathCount in deaths)
                GUILayout.Label($"Player {_deathCount.Key} : {_deathCount.Value} deaths");
    }
}
