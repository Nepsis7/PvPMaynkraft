using System;
using Unity.Netcode;
using UnityEngine;
using SF = UnityEngine.SerializeField;

[RequireComponent(typeof(Rigidbody))]
public partial class Player : NetworkBehaviour
{
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
    #endregion Syncronized
    Rigidbody rb = null;
    float camPitch = 0f;
    Camera cam = null;
    bool isGrounded = true;
    bool canGetHit = true;
    bool canJump = true;
    #endregion Fields
    #region Methods
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
    #endregion Methods
}
