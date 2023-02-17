using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using SF = UnityEngine.SerializeField;

[RequireComponent(typeof(Rigidbody))]
public partial class Player : NetworkBehaviour
{
    #region Events
    public event Action<FixedString32Bytes> OnUsernameChanged = null;
    #endregion Events
    #region Fields
    #region Serialized
    [SF] float deathHeight = -1f;
    [SF] Transform camSocket = null;
    [SF] PlayerMovementSettings movementSettings = new();
    [SF] PlayerInputSettings inputSettings = new();
    [SF] IsGroundedRayCastSettings isGroundedRayCastSettings = new();
    [SF] HitRayCastSettings hitRayCastSettings = new();
    #endregion Serialized
    #region Syncronized
    NetworkVariable<Vector3> syncedPosition = new(Vector3.zero);
    NetworkVariable<float> syncedYaw = new(0f);
    NetworkVariable<ulong> syncedPlayerID = new(0ul);
    NetworkVariable<FixedString32Bytes> username = new("");
    #endregion Syncronized
    #region Rest
    Rigidbody rb = null;
    float camPitch = 0f;
    Camera cam = null;
    bool isGrounded = true;
    bool canGetHit = true;
    bool canJump = true;
    BonusMultipliers bonusMultipliers = new();
    #endregion Rest
    #endregion Fields
    #region Accessors
    public ulong PlayerID => syncedPlayerID.Value;
    public FixedString32Bytes Username { get => username.Value; set => username.Value = value; }
    #endregion Accessors
    #region Methods
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        username.OnValueChanged += (_old, _new) => OnUsernameChanged.Invoke(_new);
        if (IsLocalPlayer)
        {
            Respawn(false);
            InitCam();
        }
        if (IsServer && PlayerManager.Instance && PlayerManager.Instance.RegisterPlayer(this, out ulong _id))
            syncedPlayerID.Value = _id;
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
    #endregion Methods
}
