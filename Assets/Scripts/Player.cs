using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : NetworkBehaviour
{
    [SerializeField] float deathHeight = -1f;
    [SerializeField] float speed = 2f;
    [SerializeField] Vector2 sensitivity = new Vector2(2f, 2f);
    [SerializeField] Vector2 camPitchMinMax = new Vector2(-70, 85);
    [SerializeField] Transform camSocket = null;
    NetworkVariable<Vector3> syncedPosition = new NetworkVariable<Vector3>(Vector3.zero);
    NetworkVariable<float> syncedYaw = new NetworkVariable<float>(0f);
    Rigidbody rb = null;
    float camPitch = 0f;
    Camera cam = null;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        if (IsLocalPlayer)
        {
            RespawnClient(false);
            InitCam();
        }
    }
    void InitCam()
    {
        cam = Camera.main;
        if (camSocket)
            cam.transform.position = transform.position;
        cam.transform.parent = transform;
        cam.transform.rotation = transform.rotation;
    }
    void Update()
    {
        if (IsLocalPlayer)
        {
            MoveClient();
            CheckDeathClient();
            HandleCamClient();
        }
        else if (IsClient)
        {
            transform.position = syncedPosition.Value;
            transform.eulerAngles = Vector3.up * syncedYaw.Value;
        }
    }
    void HandleCamClient() //cam position, rotation & player rotation
    {
        transform.eulerAngles += Vector3.up * Input.GetAxis("Mouse X") * sensitivity.x;
        Camera _cam = Camera.main;

        camPitch += -Input.GetAxis("Mouse Y") * sensitivity.y;
        camPitch = camPitch < camPitchMinMax.x ? camPitchMinMax.x : camPitch;
        camPitch = camPitch > camPitchMinMax.y ? camPitchMinMax.y : camPitch;
        _cam.transform.localEulerAngles = Vector3.right * camPitch;
        RefreshRotationServerRpc(transform.eulerAngles.y);
    }
    void MoveClient()
    {
        rb.position += Time.deltaTime * speed * (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal"));
        RefreshPositionServerRpc(transform.position);
    }
    void CheckDeathClient()
    {
        if (transform.position.y <= deathHeight)
            RespawnClient();
    }
    void RespawnClient(bool _addFall = true)
    {
        if (SpawnManager.Instance)
            transform.position = SpawnManager.Instance.GetSpawnPoint();
        RefreshPositionServerRpc(transform.position);
        if (_addFall)
            AddFallServerRpc();
    }
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
    [ServerRpc] void AddFallServerRpc() => DeathBoard.Instance?.AddFallServer(NetworkObjectId);
}
