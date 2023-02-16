using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

[RequireComponent(typeof(NetworkManager), typeof(UnityTransport))]
public class MenuGUI : MonoBehaviour
{
    NetworkManager netman = null;
    UnityTransport transport = null;
    string username = "Michel";
    private void Start()
    {
        netman = GetComponent<NetworkManager>();
        transport = GetComponent<UnityTransport>();
    }
    private void OnGUI()
    {
        //IP
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("IP");
            transport.ConnectionData.Address = GUILayout.TextField(transport.ConnectionData.Address);
            GUILayout.EndHorizontal();
        }

        //Server Listen IP
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Server Listen IP");
            transport.ConnectionData.ServerListenAddress = GUILayout.TextField(transport.ConnectionData.ServerListenAddress);
            GUILayout.EndHorizontal();
        }

        //Port
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Port");
            bool _success = ushort.TryParse(GUILayout.TextField(transport.ConnectionData.Port.ToString()), out ushort _result);
            if (_success)
                transport.ConnectionData.Port = _result;
            GUILayout.EndHorizontal();
        }

        //Username
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Username");
            username = GUILayout.TextField(username);
            GUILayout.EndHorizontal();
        }

        //Buttons
        {
            if (!string.IsNullOrWhiteSpace(username) && !username.Contains('|') && GUILayout.Button("Start Client"))
            {
                netman.StartClient();
                netman.OnClientConnectedCallback += ntm;
                Destroy(this);
            }
            if (GUILayout.Button("Start Server"))
            {
                netman.StartServer();
                Destroy(this);
            }
        }
    }
    void ntm(ulong _shlong)
    {
        Debug.Log("MANGE BIEN TES MORTS");
        UsernameManager.Instance?.RegisterUsername(username);
        netman.OnClientConnectedCallback -= ntm;
    }
}
