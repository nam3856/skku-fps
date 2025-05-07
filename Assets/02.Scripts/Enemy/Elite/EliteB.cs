using System;
using UnityEngine;

public class EliteB : MonoBehaviour
{
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

    private void Start()
    {
        StartRectIndicator();
    }
    internal void StartRectIndicator()
    {
        IsAttacking = true;
        //EnemyController.Agent.ResetPath();
        //EnemyController.Agent.enabled = false;
        TrailRendererObj.SetActive(true);
        Indicator.SetActive(true);
    }
    
    internal void StartRectangleAttack()
    {
        Step = 0;
        AttackStep = 0;
        TestAnimator.SetTrigger("AreaAttack");
        //EnemyController.Animator.SetTrigger("AreaAttack");
    }

    private void Update()
    {
        TestController.Move(transform.forward * Time.deltaTime * AttackPosition[Step]);
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
}
