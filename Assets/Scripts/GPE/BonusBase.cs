using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BonusBase : MonoBehaviour
{
    public event Action<Player> OnPlayerEnter = null;
    [SerializeField] LayerMask playerMask = 1 << 6;
    [SerializeField] BoxCollider ownCollider = null;
    [SerializeField] BonusSettings settings;
    [SerializeField] GameObject bonusRenderer = null;
    [SerializeField] bool drawGizmos = false;

    float yOffset;
    float angle = 0;
    private void Start() => Init();
    private void Update()
    {
        BonusAnimation();
    }
    void BonusAnimation()
    {
        if (!bonusRenderer || !bonusRenderer.activeSelf)
            return;
        angle += Time.deltaTime * 100;
        angle = angle > 360 ? 0 : angle;

        float _y = yOffset + Mathf.Sin(angle * Mathf.Deg2Rad);
        float _x = transform.position.x;
        float _z = transform.position.z;
        transform.position = new Vector3(_x, _y, _z);

        float _roll = bonusRenderer.transform.eulerAngles.x;
        float _pitch = angle;
        float _yaw = bonusRenderer.transform.eulerAngles.z;
        bonusRenderer.transform.eulerAngles = new Vector3(_roll, _pitch,_yaw);
    }
    void Init()
    {
        if (!ownCollider)
            ownCollider = GetComponent<BoxCollider>();
        if (!settings || !bonusRenderer)
            return;
        yOffset = transform.position.y;
        OnPlayerEnter += (player) =>
        {
            settings.BonusBehaviour(player);
            DesactiveBonus();
            Invoke(nameof(ReSpawnBonus), settings.TimeRespawnBonus);
        };
    }
    void DesactiveBonus() 
    {
        ownCollider.enabled = false;
        bonusRenderer.SetActive(false);
    }
    void ReSpawnBonus()
    {
        ownCollider.enabled = true;
        bonusRenderer.SetActive(true);
    }
    private void OnTriggerEnter(Collider _collider)
    {
        if ((1 << _collider.gameObject.layer) != playerMask)
            return;
        Player _player = _collider.GetComponent<Player>();
        OnPlayerEnter?.Invoke(_player);
    }
    private void OnDrawGizmos()
    {
        if (!ownCollider || !drawGizmos)
            return;
        Gizmos.color = settings.BonusColor;
        Gizmos.DrawWireCube(ownCollider.bounds.center, ownCollider.bounds.size);
    }
}
