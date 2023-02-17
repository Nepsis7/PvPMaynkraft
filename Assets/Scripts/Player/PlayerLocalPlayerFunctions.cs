using Unity.Netcode;
using UnityEngine;

public partial class Player : NetworkBehaviour
{
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
    public void SetUsernameIfLocalPlayer(string _username)
    {
        if (IsLocalPlayer)
            SetUsernameServerRpc(_username);
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
        transform.eulerAngles += Input.GetAxis(inputSettings.YawAxis) * movementSettings.Sensitivity.x * Vector3.up;
        Camera _cam = Camera.main;

        camPitch += -Input.GetAxis(inputSettings.PitchAxis) * movementSettings.Sensitivity.y;
        camPitch = camPitch < movementSettings.CamPitchMinMax.x ? movementSettings.CamPitchMinMax.x : camPitch;
        camPitch = camPitch > movementSettings.CamPitchMinMax.y ? movementSettings.CamPitchMinMax.y : camPitch;
        _cam.transform.localEulerAngles = Vector3.right * camPitch;
        RefreshRotationServerRpc(transform.eulerAngles.y);
    }
    void Move()
    {
        rb.position += Move_Speed * Time.deltaTime * Move_Direction;
        TryJump();
        RefreshPositionServerRpc(transform.position);
    }
    #region Move() subAccessors
    float Move_Speed => movementSettings.Speed * Move_TotalSpeedFactor;
    float Move_TotalSpeedFactor => Move_InAirFactor * Move_SprintingFactor;
    float Move_InAirFactor => isGrounded ? 1 : movementSettings.InAirSpeedFactor;
    float Move_SprintingFactor => Input.GetAxis(inputSettings.SprintAxis) > 0 ? movementSettings.SprintSpeedFactor : 1;
    Vector3 Move_Direction => (Move_Forward + Move_Strafe).normalized;
    Vector3 Move_Forward => transform.forward * Input.GetAxis(inputSettings.ForwardAxis);
    Vector3 Move_Strafe => transform.right * Input.GetAxis(inputSettings.StrafeAxis);
    #endregion Move() subAccessors
    void TryJump()
    {
        if (!canJump || !isGrounded || Input.GetAxis(inputSettings.JumpAxis) == 0)
            return;
        canJump = false;
        rb.AddForce(Vector3.up * movementSettings.JumpForce, ForceMode.VelocityChange);
        Invoke(nameof(AllowJumping), movementSettings.JumpDelay);
    }
    void AllowJumping() => canJump = true;
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
        if (!camSocket || !cam || Input.GetAxis(inputSettings.HitAxis) == 0)
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
        HitPlayerServerRpc(_hitPlayer.PlayerID, _kbDir.normalized);
    }
    void RefreshIsGrounded() => isGrounded = Physics.Raycast(transform.position + Vector3.up * isGroundedRayCastSettings.HeightOffset, -Vector3.up, isGroundedRayCastSettings.Length, ~gameObject.layer);
}
