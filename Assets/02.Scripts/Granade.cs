using UnityEngine;

public class Granade : MonoBehaviour
{
    [SerializeField] private GameObject _effect;
    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private int explosionDamage = 10;
    [SerializeField] private float explosionKnockback = 10f;
    [SerializeField] private LayerMask enemyLayer;    // 인스펙터에서 “Enemy” 레이어만 체크

    private GranadePool _pool;

    private void Awake()
    {
        _pool = FindFirstObjectByType<GranadePool>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(_effect, transform.position, Quaternion.identity);

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            explosionRadius,
            enemyLayer.value
        );

        foreach (var hit in hits)
        {
            var dmgable = hit.GetComponent<IDamageable>();
            if (dmgable != null)
            {
                var dmg = new Damage
                {
                    amount = explosionDamage,
                    type = DamageType.Explosion,
                    origin = transform.position,
                    knockbackForce = explosionKnockback
                };
                dmgable.TakeDamage(dmg);
            }
        }

        _pool.ReturnGrenade(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
