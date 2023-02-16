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
}
