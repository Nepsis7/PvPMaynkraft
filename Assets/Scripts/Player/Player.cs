using System;
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
public partial class Player : NetworkBehaviour
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
    [SF] float hitDelay = .25f;
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
