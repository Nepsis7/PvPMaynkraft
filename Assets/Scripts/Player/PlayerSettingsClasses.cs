using System;
using System.Collections;
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
    [SF] float inAirSpeedMultiplier = .5f;
    [SF] float sprintSpeedMultiplier = 2f;
    [SF] float jumpForce = 5f;
    [SF] Vector2 sensitivity = new(2f, 2f);
    [SF] Vector2 camPitchMinMax = new(-70, 90);
    [SF] float hitDelay = 1f;
    [SF] float jumpDelay = .5f;
    [SF] KnockBackMultipliers kbMultipliers = new();
    public float Speed => speed;
    public float InAirSpeedMultiplier => inAirSpeedMultiplier;
    public float SprintSpeedMultiplier => sprintSpeedMultiplier;
    public float JumpForce => jumpForce;
    public Vector2 Sensitivity => sensitivity;
    public Vector2 CamPitchMinMax => camPitchMinMax;
    public float HitDelay => hitDelay;
    public float JumpDelay => jumpDelay;
    public KnockBackMultipliers KbMultipliers => kbMultipliers;
}

class BonusMultipliers
{
    public float Force = 1f;
    public float Speed = 1f;
    public float JumpForce = 1f;
    public float Get(PlayerBonusType _type)
    {
        switch (_type)
        {
            case PlayerBonusType.Force:
                return Force;
            case PlayerBonusType.Speed:
                return Speed;
            case PlayerBonusType.JumpForce:
                return JumpForce;
        }
        return 0f; //c# il est con
    }
    public float Set(PlayerBonusType _type, float _value)
    {
        switch (_type)
        {
            case PlayerBonusType.Force:
                return (Force = _value);
            case PlayerBonusType.Speed:
                return Speed = _value;
            case PlayerBonusType.JumpForce:
                return JumpForce = _value;
        }
        return 0f; //c# il est toujours con
    }
    public void ResetAfterDuration(PlayerBonusType _type, float _duration, MonoBehaviour _coroutineHost) => _coroutineHost?.StartCoroutine(ResetCoroutine(_type, _duration));
    IEnumerator ResetCoroutine(PlayerBonusType _type, float _duration)
    {
        yield return new WaitForSeconds(_duration);
        Set(_type, 1);
    }
}

public enum PlayerBonusType
{
    Force,
    Speed,
    JumpForce,
}
