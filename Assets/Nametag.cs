using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class Nametag : NetworkBehaviour
{
    void Start()
    {
        UsernameManager _um = UsernameManager.Instance;
        if (IsLocalPlayer && _um)
            _um.OnUsernameSet += SetUsernameServerRpc;
    }
    private void Update()
    {
        Vector3 _lookAt = Quaternion.LookRotation(transform.position - Camera.main.transform.position, Vector3.up).eulerAngles;
        if (transform.parent)
            transform.parent.eulerAngles = Vector3.up * _lookAt.y;
    }
    void SetUsername(string _username) => GetComponent<TMP_Text>().text = _username;
    [ServerRpc]
    void SetUsernameServerRpc(string _username)
    {
        SetUsername(_username);
        SetUsernameClientRpc(_username);
    }
    [ClientRpc] void SetUsernameClientRpc(string _username) => SetUsername(_username);
}
