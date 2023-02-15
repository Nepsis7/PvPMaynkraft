using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

[RequireComponent(typeof(NetworkManager), typeof(UnityTransport))]
public class MenuGUI : MonoBehaviour
{
    NetworkManager nm = null;
    UnityTransport transport = null;
    bool started = false;
    private void Start()
    {
        nm = GetComponent<NetworkManager>();
        transport = GetComponent<UnityTransport>();
    }
    private void OnGUI()
    {
        if (started)
            return;
        GUILayout.BeginHorizontal();
        GUILayout.Label("IP");
        transport.ConnectionData.Address = GUILayout.TextField(transport.ConnectionData.Address);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Server Listen IP");
        transport.ConnectionData.ServerListenAddress = GUILayout.TextField(transport.ConnectionData.ServerListenAddress);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Port");
        bool _success = ushort.TryParse(GUILayout.TextField(transport.ConnectionData.Port.ToString()), out ushort _result);
        if (_success)
            transport.ConnectionData.Port = _result;
        GUILayout.EndHorizontal();
        if(GUILayout.Button("Start Client"))
        {
            nm.StartClient();
            started = true;
        }
        else if (GUILayout.Button("Start Server"))
        {
            nm.StartServer();
            started = true;
        }
        else if (GUILayout.Button("Start Host"))
        {
            nm.StartHost();
            started = true;
        }
    }
}
