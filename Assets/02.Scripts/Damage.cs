using UnityEngine;

public enum DamageType
{
    Normal,
    Explosion,
    Count
}
public struct Damage
{
    public float amount;
    public DamageType type;
    public Vector3 origin;
    public float knockbackForce;
}

public interface IDamageable
{
    void TakeDamage(Damage dmg);
}