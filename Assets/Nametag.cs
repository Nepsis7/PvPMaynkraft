using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class Nametag : MonoBehaviour
{
    [SerializeField] Player player = null;
    TMP_Text text = null;
    void Start()
    {
        text = GetComponent<TMP_Text>();
        if (player)
        {
            text.text = player.Username.ToString();
            player.OnUsernameChanged += (_username) => text.text = _username.ToString();
        }
    }
    private void Update()
    {
        if (!transform.parent)
            return;
        Vector3 _lookAt = Quaternion.LookRotation(transform.position - Camera.main.transform.position, Vector3.up).eulerAngles;
        transform.parent.eulerAngles = Vector3.up * _lookAt.y;
    }
}
