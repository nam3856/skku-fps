// MeleeService.cs
using System.Collections;
using UnityEngine;

public class MeleeService : MonoBehaviour
{
    [Header("근접 공격 설정")]
    [SerializeField] private float attackRadius = 2f;           // 오버랩 스피어 반경
    [SerializeField] private float attackAngle = 60f;          // 부채꼴 전체 각도
    [SerializeField] private LayerMask targetMask;                  // 적 레이어
    [SerializeField] private Transform attackPoint;                 // 공격 기준 위치 (예: 플레이어 허리나 검 끝)
    private Animator[] animators;                    // 검 휘두르는 애니메이터
    private PlayerCombatDataSO _data;
    private float _meleeCooldown => _data.MeleeCooldown;
    public bool IsSwinging { get; private set; }

    private void Start()
    {
        _data = GetComponent<PlayerCombatController>().combatDataSO;
        animators = GetComponent<PlayerCombatController>().animators;
    }

    public void Attack()
    {
        if (IsSwinging) return;
        IsSwinging = true;
        StartCoroutine(Swing());
    }
    private IEnumerator Swing()
    {
        foreach (var animator in animators)
        {
            if (animator != null)
                animator.SetTrigger("Swing");
        }


        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRadius, targetMask);

        foreach (var col in hits)
        {
            Vector3 toTarget = (col.transform.position - attackPoint.position).normalized;
            float halfAngle = attackAngle * 0.5f;
            if (Vector3.Angle(attackPoint.forward, toTarget) > halfAngle)
                continue;

            var dmgable = col.GetComponent<IDamageable>();
            if (dmgable != null)
            {
                var dmg = new Damage
                {
                    amount = _data.Damage,
                    type = DamageType.Normal,
                    origin = attackPoint.position,
                    knockbackForce = _data.KnockbackByType[(int)DamageType.Normal]
                };
                dmgable.TakeDamage(dmg);
            }
        }

        yield return new WaitForSeconds(_meleeCooldown);
        IsSwinging = false;
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);

        Vector3 rightDir = Quaternion.Euler(0, attackAngle * 0.5f, 0) * attackPoint.forward;
        Vector3 leftDir = Quaternion.Euler(0, -attackAngle * 0.5f, 0) * attackPoint.forward;
        Gizmos.DrawLine(attackPoint.position, attackPoint.position + rightDir * attackRadius);
        Gizmos.DrawLine(attackPoint.position, attackPoint.position + leftDir * attackRadius);
    }
}
