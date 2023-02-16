using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Globalization;

public class DeathBoard : NetworkSingleton<DeathBoard>
{
    Dictionary<ulong, int> deaths = new Dictionary<ulong, int>();
    public void AddFallServer(ulong _playerID)
    {
        AddFall(_playerID);
        AddFallClientRpc(_playerID);
    }
    [ClientRpc] void AddFallClientRpc(ulong _playerID) => AddFall(_playerID);
    void AddFall(ulong _playerID)
    {
        if (!deaths.ContainsKey(_playerID))
            deaths.Add(_playerID, 0);
        deaths[_playerID]++;
    }
    private void OnGUI()
    {
        foreach (KeyValuePair<ulong, int> _deathCount in deaths)
            GUILayout.Label($"Player {_deathCount.Key} : {_deathCount.Value} deaths");
    }
}
