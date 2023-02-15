using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : NetworkBehaviour
{
    [SerializeField] float deathHeight = -1f;
    NetworkVariable<Vector3> syncedPosition = new NetworkVariable<Vector3>(Vector3.zero);
    Rigidbody rb = null;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        if (IsClient)
            RespawnClient(false);
    }
    void Update()
    {
        if (IsLocalPlayer)
        {

            MoveClient();
            CheckDeathClient();
        }
        else if (IsClient)
            transform.position = syncedPosition.Value;
    }
    void MoveClient()
    {
        rb.position += Time.deltaTime * 2 * (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal"));
        RefreshPositionServerRpc(transform.position);
    }
    void CheckDeathClient()
    {
        if (transform.position.y <= deathHeight)
        {
            RespawnClient();
            Debug.Log("DEATH");
        }

    }
    void RespawnClient(bool _addFall = true)
    {
        if (SpawnManager.Instance)
            transform.position = SpawnManager.Instance.GetSpawnPoint();
        RefreshPositionServerRpc(transform.position);
        //if (_addFall)
        //    AddFallServerRpc();
    }
    [ServerRpc]
    void RefreshPositionServerRpc(Vector3 _position)
    {
        transform.position = _position;
        syncedPosition.Value = _position;
    }
    [ServerRpc]
    void AddFallServerRpc() => DeathBoard.Instance?.AddFallServer(NetworkObjectId);
}
