using System.Collections;
using UnityEngine;

public class Barrel : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _explosionRadius;
    [SerializeField] private LayerMask _dmgableLayer;
    [SerializeField] private float _explosionDamage;
    [SerializeField] private float _explosionKnockback;
    [SerializeField] private float _delayTime;
    [SerializeField] private GameObject _explodeParticle;
    [SerializeField] private GameObject _fireParticle;
    private bool _isExploded;
    public void TakeDamage(Damage dmg)
    {
        _currentHealth = Mathf.Max(0, _currentHealth - dmg.amount);

        if (_currentHealth == 0)
        {
            if (_isExploded) return;
            _isExploded = true;
            Explode();
            GetComponent<Rigidbody>().AddForce(
                ((transform.position - dmg.origin).normalized + Vector3.up) * dmg.knockbackForce * 10,
                ForceMode.Impulse);
        }
        else
        {
            _fireParticle.SetActive(true);
        }
    }
    void Start()
    {
        _currentHealth = _maxHealth;
    }
    
    private void Explode()
    {
        HitEffectPool.Instance.ReturnAllChildrenUnder(transform);
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            _explosionRadius,
            _dmgableLayer.value
        );
        Instantiate(_explodeParticle, transform.position, Quaternion.identity);
        foreach (var hit in hits)
        {
            var dmgable = hit.GetComponent<IDamageable>();
            if (dmgable != null)
            {
                var dmg = new Damage
                {
                    amount = _explosionDamage,
                    type = DamageType.Explosion,
                    origin = transform.position,
                    knockbackForce = _explosionKnockback
                };
                dmgable.TakeDamage(dmg);
            }

        }

        Destroy(gameObject, _delayTime);
    }

    private IEnumerator DeleteAfterExplosion()
    {
        yield return new WaitForSeconds(_delayTime);
    }
}
