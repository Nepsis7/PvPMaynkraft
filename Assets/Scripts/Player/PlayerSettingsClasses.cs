using System;
using UnityEngine;
using SF = UnityEngine.SerializeField;

[Serializable]
class PlayerInputSettings
{
    [SF] string forwardAxis = "Vertical";
    [SF] string strafeAxis = "Horizontal";
    [SF] string jumpAxis = "Jump";
    [SF] string sprintAxis = "Sprint";
    [SF] string hitAxis = "Fire1";
    [SF] string yawAxis = "Mouse X";
    [SF] string pitchAxis = "Mouse Y";
    public string ForwardAxis => forwardAxis;
    public string StrafeAxis => strafeAxis;
    public string JumpAxis => jumpAxis;
    public string SprintAxis => sprintAxis;
    public string HitAxis => hitAxis;
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

[Serializable]
class PlayerMovementSettings
{
    [SF] float speed = 2f;
    [SF] float inAirSpeedFactor = .5f;
    [SF] float sprintSpeedFactor = 2f;
    [SF] float jumpForce = 5f;
    [SF] Vector2 sensitivity = new(2f, 2f);
    [SF] Vector2 camPitchMinMax = new(-70, 90);
    [SF] float hitDelay = 1f;
    [SF] float jumpDelay = .5f;
    [SF] KnockBackMultipliers kbMultipliers = new();
    public float Speed => speed;
    public float InAirSpeedFactor => inAirSpeedFactor;
    public float SprintSpeedFactor => sprintSpeedFactor;
    public float JumpForce => jumpForce;
    public Vector2 Sensitivity => sensitivity;
    public Vector2 CamPitchMinMax => camPitchMinMax;
    public float HitDelay => hitDelay;
    public float JumpDelay => jumpDelay;
    public KnockBackMultipliers KbMultipliers => kbMultipliers;
}
