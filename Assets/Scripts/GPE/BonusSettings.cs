using UnityEngine;

[CreateAssetMenu(fileName = "BonusSettingsDefault", menuName = "GPE/Bonus")]
public class BonusSettings : ScriptableObject
{
    [SerializeField, Range(0, 100)] int multPercent = 0;
    [SerializeField, Range(1, 5)] float timeBonus = 1;
    [SerializeField, Range(1, 20)] float timeRespawnBonus = 5;
    [SerializeField] PlayerBonusType type;
    [SerializeField] Color bonusColor = Color.white;
    public int MultPercent => multPercent;
    public float TimeBonus => timeBonus;
    public float TimeRespawnBonus => timeRespawnBonus;
    public Color BonusColor => bonusColor;
    public void BonusBehaviour(Player _player)
    {
        _player.AddBonusIfLocalPlayer(type, multPercent / 100f, timeBonus);
    }
}
