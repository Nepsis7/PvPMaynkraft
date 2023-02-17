using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

class PlayerRegistry
{
    List<Player> players = new();
    List<ulong> ids = new();
    ulong lastUsedIndex = 0;
    public ulong LastUsedIndex => lastUsedIndex;
    public ulong this[Player _player]
    {
        get
        {
            for (int i = 0; i < players.Count; i++)
                if (players[i] == _player)
                    return ids[i];
            return 0;
        }
        set
        {
            for (int i = 0; i < players.Count; i++)
                if (players[i] == _player)
                {
                    ids[i] = value;
                    return;
                }
        }
    }
    public Player this[ulong _id]
    {
        get
        {
            for (int i = 0; i < players.Count; i++)
                if (ids[i] == _id)
                    return players[i];
            return null;
        }
        set
        {
            for (int i = 0; i < players.Count; i++)
                if (ids[i] == _id)
                {
                    players[i] = value;
                    return;
                }
        }
    }
    public bool AddPlayer(Player _player)
    {
        if (HasPlayer(_player))
            return false;
        lastUsedIndex++;
        players.Add(_player);
        ids.Add(lastUsedIndex);
        return true;
    }
    public bool HasPlayer(Player _player) => players.Contains(_player);
    public bool HasID(ulong _id) => ids.Contains(_id);

}

public class PlayerManager : Singleton<PlayerManager>
{
    PlayerRegistry registry = new ();
    public bool RegisterPlayer(Player _player, out ulong _id)
    {
        _id = 0ul;
        if(!registry.AddPlayer(_player))
            return false;
        _id =registry.LastUsedIndex; 
        return true;
    }
    public Player this[ulong _id] => registry[_id];
    public ulong this[Player _player] => registry[_player];
}
