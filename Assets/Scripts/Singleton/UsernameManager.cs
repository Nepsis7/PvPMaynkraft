using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class UsernameManager : NetworkSingleton<UsernameManager>
{
    public const char USERNAME_SEPARATOR = '|';
    public event Action<string> OnUsernameSet = null;
    string registeredUsernames = "";
    string username = "Michel";
    bool refreshed = false;
    public string Username => username;
    public void RegisterUsername(string _username)
    {
        Debug.Log($"REGISTERING USERNAME {_username}");
        refreshed = false;
        RequestUsernamesRefreshServerRpc();
        StartCoroutine(RegisterUsernameCoroutine(_username));
    }
    IEnumerator RegisterUsernameCoroutine(string _username)
    {
        while (!refreshed)
            yield return null;
        username = _username;
        int _n = 2;
        while (registeredUsernames.Split(USERNAME_SEPARATOR).Contains(username))
        {
            username = _username + _n.ToString();
            _n++;
        }
        RegisterUsernameServerRpc(username);
        OnUsernameSet.Invoke(username);
    }
    [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
    void RegisterUsernameServerRpc(string _username)
    {
        if (string.IsNullOrEmpty(registeredUsernames))
            registeredUsernames = _username;
        else
            registeredUsernames += USERNAME_SEPARATOR + username;
    }
    [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
    void RequestUsernamesRefreshServerRpc()
    {
        Debug.Log("Requested A Refresh");
        RefreshRegisteredUsernamesClientRpc(registeredUsernames);
    }
    [ClientRpc]
    void RefreshRegisteredUsernamesClientRpc(string _usernames)
    {
        registeredUsernames = _usernames;
        refreshed = true;
    }
    //string AllInOne(List<string> _all)
    //{
    //    string _one = "";
    //    foreach (string _element in _all)
    //        _one += _element + '|';
    //    return _one;
    //}
    //List<string> AllFromOne(string _one) => _one.Split('|').ToList();
}
