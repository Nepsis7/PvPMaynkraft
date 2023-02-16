using System;
using Unity.Netcode;
using UnityEngine;
using SF = UnityEngine.SerializeField;

[Serializable]
class PlayerInputSettings
{
    [SF] KeyCode hitKey = KeyCode.Mouse0;
    [SF] string forwardAxis = "Vertical";
    [SF] string strafeAxis = "Horizontal";
    [SF] string yawAxis = "Mouse X";
    [SF] string pitchAxis = "Mouse Y";
    public KeyCode HitKey => hitKey;
    public string ForwardAxis => forwardAxis;
    public string StrafeAxis => strafeAxis;
    public string YawAxis => yawAxis;
    public string PitchAxis => pitchAxis;
}

[RequireComponent(typeof(Rigidbody))]
public class Player : NetworkBehaviour
{
    #region Fields
    #region Serialized
    [SF] float deathHeight = -1f;
    [SF] float speed = 2f;
    [SF] Vector2 sensitivity = new Vector2(2f, 2f);
    [SF] Vector2 camPitchMinMax = new Vector2(-70, 85);
    [SF] Transform camSocket = null;
    [SF] PlayerInputSettings inputSettings = new PlayerInputSettings();
    #endregion Serialized
    #region Syncronized
    NetworkVariable<Vector3> syncedPosition = new NetworkVariable<Vector3>(Vector3.zero);
    NetworkVariable<float> syncedYaw = new NetworkVariable<float>(0f);
    #endregion Syncronized
    Rigidbody rb = null;
    float camPitch = 0f;
    Camera cam = null;
    #endregion Fields
    #region Methods
    #region UnityStuff
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
    #endregion UnityStuff
    #region LocalPlayer
    public void SetPositionIfLocalPlayer(Vector3 _position)
    {
        if (!IsLocalPlayer)
            return;
        transform.position = _position;
        RefreshPositionServerRpc(_position);
    }
    void InitCam()
    {
        cam = Camera.main;
        if (camSocket)
            cam.transform.position = transform.position;
        cam.transform.parent = transform;
        cam.transform.rotation = transform.rotation;
    }
    void HandleCamClient() //cam position, rotation & player rotation
    {
        transform.eulerAngles += Vector3.up * Input.GetAxis(inputSettings.YawAxis) * sensitivity.x;
        Camera _cam = Camera.main;

        camPitch += -Input.GetAxis(inputSettings.PitchAxis) * sensitivity.y;
        camPitch = camPitch < camPitchMinMax.x ? camPitchMinMax.x : camPitch;
        camPitch = camPitch > camPitchMinMax.y ? camPitchMinMax.y : camPitch;
        _cam.transform.localEulerAngles = Vector3.right * camPitch;
        RefreshRotationServerRpc(transform.eulerAngles.y);
    }
    void MoveClient()
    {
        rb.position += Time.deltaTime * speed * (transform.forward * Input.GetAxis(inputSettings.ForwardAxis) + transform.right * Input.GetAxis(inputSettings.StrafeAxis));
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
    #endregion LocalPlayer
    #region ServerRpc
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
    #endregion ServerRpc
    #endregion Methods
}
