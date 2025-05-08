using System;
using UnityEngine;

public class EliteB : MonoBehaviour
{
    public enum AttackType
    {
        Attack,
        AreaAttack,
    }
    public GameObject TrailRendererObj;
    public GameObject Indicator;
    public AudioSource AudioSource;
    public AudioClip[] AudioClips;
    public GameObject[] ShotEffects;
    public EnemyController EnemyController;
    public bool IsAttacking;
    public float[] AttackPosition;
    public float[] AttackDamage;
    public int Step;
    public int AttackStep;
    public float[] AttackAngles;
    public float AttackRadius;

    public CharacterController TestController;
    public LayerMask TestLayer;
    public Animator TestAnimator;

    internal void StartAreaAttackRectIndicator()
    {
        IsAttacking = true;
        //EnemyController.Agent.ResetPath();
        //EnemyController.Agent.enabled = false;
        TrailRendererObj.SetActive(true);
        Indicator.SetActive(true);
        Indicator.GetComponent<RectAreaIndicator>().length = 0.5f;
    }

    internal void StartDashAttackRectIndicator()
    {

    }
    
    internal void StartAttack(int index)
    {
        AttackType type = (AttackType)index;
        Step = 0;
        AttackStep = 0;
        TestAnimator.applyRootMotion = true;
        TestAnimator.SetTrigger(type.ToString());
        //EnemyController.Animator.SetTrigger(type.ToString());
    }

    public void EndAttack()
    {
        TestAnimator.applyRootMotion = false;
        TestAnimator.SetTrigger("EndAttack");
    }

    public void AttackMove()
    {
        
        //EnemyController.Controller.Move(EnemyController.transform.position + AttackPosition[Step]);
        Step++;
    }

    public void Attack()
    {
        PerformAreaDamage(transform.position, transform.forward, AttackAngles[AttackStep], AttackRadius);
        AttackStep++;
    } 

    private void PerformAreaDamage(Vector3 origin, Vector3 forward, float angle, float radius)
    {
        Collider[] hits = Physics.OverlapSphere(origin, radius, TestLayer);
        //Collider[] hits = Physics.OverlapSphere(origin, radius, EnemyController.enemyLayer);
        foreach (var hit in hits)
        {
            Vector3 toTarget = (hit.transform.position - origin).normalized;
            if (Vector3.Angle(forward, toTarget) <= angle * 0.5f)
            {
                if (hit.TryGetComponent(out IDamageable target))
                {
                    target.TakeDamage(new Damage
                    {
                        amount = AttackDamage[AttackStep],
                        origin = origin,
                        type = DamageType.Normal
                    });
                }
            }
        }
    }

    void OnAnimatorMove()
    {
        if (TestAnimator.applyRootMotion)
        {
            Vector3 deltaPosition = TestAnimator.deltaPosition;
            TestController.Move(deltaPosition);
        }
        //if (EnemyController.Animator.applyRootMotion)
        //{
        //    Vector3 deltaPosition = EnemyController.Animator.deltaPosition;
        //    EnemyController.Controller.Move(deltaPosition);
        //}
    }
}
