using System;
using System.Security.Cryptography;
using Unity.Netcode;
using UnityEngine;
using SF = UnityEngine.SerializeField;

#region Settings Classes
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

[Serializable]
class HitRayCastSettings
{
    [SF] float forwardOffset = .4f;
    [SF] float length = 2.6f;
    public float ForwardOffset => forwardOffset;
    public float Length => length;
}

[Serializable]
class IsGroundedRayCastSettings
{
    [SF] float heightOffset = -.98f;
    [SF] float length = .04f;
    public float HeightOffset => heightOffset;
    public float Length => length;
}

[Serializable]
class KnockBackMultipliers
{
    [SF] float vertical = 1f;
    [SF] float horizontal = 1f;
    public float Vertical => vertical;
    public float Horizontal => horizontal;
}
#endregion Settings Classes

[RequireComponent(typeof(Rigidbody))]
public class Player : NetworkBehaviour
{
    #region Fields
    #region Serialized
    [SF] float deathHeight = -1f;
    [SF] float speed = 2f;
    [SF] float inAirSpeedFactor = .5f;
    [SF] Vector2 sensitivity = new(2f, 2f);
    [SF] Vector2 camPitchMinMax = new(-70, 85);
    [SF] Transform camSocket = null;
    [SF] PlayerInputSettings inputSettings = new();
    [SF] IsGroundedRayCastSettings isGroundedRayCastSettings = new();
    [SF] HitRayCastSettings hitRayCastSettings = new();
    [SF] KnockBackMultipliers kbMultipliers = new();
    #endregion Serialized
    #region Syncronized
    NetworkVariable<Vector3> syncedPosition = new(Vector3.zero);
    NetworkVariable<float> syncedYaw = new(0f);
    #endregion Syncronized
    Rigidbody rb = null;
    float camPitch = 0f;
    Camera cam = null;
    bool isGrounded = true;
    #endregion Fields
    #region Methods
    #region UnityStuff
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (IsLocalPlayer)
        {
            Respawn(false);
            InitCam();
        }
    }
    void Update()
    {
        if (IsLocalPlayer)
        {
            RefreshIsGrounded();
            Move();
            CheckDeath();
            HandleCam();
            TryHitPlayers();
        }
        else if (IsClient)
        {
            transform.position = syncedPosition.Value;
            transform.eulerAngles = Vector3.up * syncedYaw.Value;
        }
    }
    #endregion UnityStuff
    #region LocalPlayer
    public void AddForceIfLocalPlayer(Vector3 _force)
    {
        if (IsLocalPlayer)
            rb.AddForce(_force, ForceMode.Impulse);
    }
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
    void HandleCam() //cam position, cam rotation & player rotation
    {
        transform.eulerAngles += Input.GetAxis(inputSettings.YawAxis) * sensitivity.x * Vector3.up;
        Camera _cam = Camera.main;

        camPitch += -Input.GetAxis(inputSettings.PitchAxis) * sensitivity.y;
        camPitch = camPitch < camPitchMinMax.x ? camPitchMinMax.x : camPitch;
        camPitch = camPitch > camPitchMinMax.y ? camPitchMinMax.y : camPitch;
        _cam.transform.localEulerAngles = Vector3.right * camPitch;
        RefreshRotationServerRpc(transform.eulerAngles.y);
    }
    void Move()
    {
        rb.position += Time.deltaTime * speed * (isGrounded ? 1 : inAirSpeedFactor) * (transform.forward * Input.GetAxis(inputSettings.ForwardAxis) + transform.right * Input.GetAxis(inputSettings.StrafeAxis));
        RefreshPositionServerRpc(transform.position);
    }
    void CheckDeath()
    {
        if (transform.position.y <= deathHeight)
            Respawn();
    }
    void Respawn(bool _addFall = true)
    {
        if (SpawnManager.Instance)
            transform.position = SpawnManager.Instance.GetSpawnPoint();
        rb.velocity = Vector3.zero;
        RefreshPositionServerRpc(transform.position);
        if (_addFall)
            AddFallServerRpc();
    }
    void TryHitPlayers()
    {
        if (!camSocket || !cam || !Input.GetKeyDown(inputSettings.HitKey))
            return;
        Vector3 _rayStartPos = camSocket.position + cam.transform.forward * hitRayCastSettings.ForwardOffset;
        Vector3 _rayDirection = cam.transform.forward;
        if (!Physics.Raycast(_rayStartPos, _rayDirection, out RaycastHit _hit, hitRayCastSettings.Length))
            return;
        Player _hitPlayer = _hit.collider.GetComponent<Player>();
        if (!_hitPlayer)
            return;
        Vector3 _kbDir = cam.transform.forward;
        _kbDir.y = 1;
        HitPlayerServerRpc(_hitPlayer.NetworkObjectId, _kbDir.normalized);
    }
    void RefreshIsGrounded() => isGrounded = Physics.Raycast(transform.position + Vector3.up * isGroundedRayCastSettings.HeightOffset, -Vector3.up, isGroundedRayCastSettings.Length, ~gameObject.layer);
    #endregion LocalPlayer
    #region ClientRpc
    [ClientRpc]
    void KnockBackIfLocalPlayerClientRpc(Vector3 _dirNormalized)
    {
        if (IsLocalPlayer)
            rb.AddForce(4*new Vector3(_dirNormalized.x * kbMultipliers.Horizontal, _dirNormalized.y * kbMultipliers.Vertical, _dirNormalized.z * kbMultipliers.Horizontal), ForceMode.Impulse); ;
    }
    #endregion ClientRpc
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
    [ServerRpc] void HitPlayerServerRpc(ulong _playerID, Vector3 _dirNormalized) => NetworkManager.SpawnManager.SpawnedObjects[_playerID].GetComponent<Player>().KnockBackIfLocalPlayerClientRpc(_dirNormalized);
    #endregion ServerRpc
    #endregion Methods
}
