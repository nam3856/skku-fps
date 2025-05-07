using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCombatData", menuName = "ScriptableObjects/PlayerCombatData")]
public class PlayerCombatDataSO : ScriptableObject
{
    [Header("탄약/수류탄")]
    public int MaxAmmo = 50;
    public int MaxGrenade = 3;

    [Header("대미지/넉백")]
    public float Damage = 1f;
    public float[] KnockbackByType = new float[(int)DamageType.Count];

    [Header("공격 쿨타임")]
    public float MeleeCooldown = 1f;
}
