using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
public class EnemyController : MonoBehaviour, IDamageable
{
    [HideInInspector] public CharacterController Controller;
    
    [HideInInspector] public EnemyMovement Movement;
    [HideInInspector] public EnemyHealth Health;
    [HideInInspector] public EnemyStateMachine StateMachine;
    [HideInInspector] public NavMeshAgent Agent;

    public const float Gravity = 10f;

    public EnemyType Type;

    [Header("References")]
    public Transform Player;
    public ParticleSystem HitEffect;
    public GameObject DeathEffect;
    public EnemyUI UI_Enemy;
    public Animator Animator;

    [Header("Settings")]
    [Header("Distance Settings")]
    public float FindDistance;
    public float ReturnDistance;
    public float AttackDistance;

    public LayerMask GroundLayer;

    [Header("Move & Patrol Settings")]
    public float MoveSpeed;
    public int PatrolCount;
    public float PatrolSpeed;
    public float PatrolThreshold;
    public float MinPatrolRange;
    public float MaxPatrolRange;

    [Header("Attack Settings")]
    public float AttackCooldown;
    public float SkillCooldown;
    public float AttackRange;
    public float AttackDamage;
    public LayerMask enemyLayer;
    public float KnockbackStrength;
    public float KnockbackDuration;
    public float StunDuration;
    public float AggressiveTraceTime;
    public float AggressiveTraceDistance;

    public float MaxHealth;

    internal float LastAttack;
    internal float LastSkill;

    [Header("Optional (for elite enemies)")]
    public EliteA EliteAScript; 
    public EliteB EliteBScript;

    public Vector3 StartPosition;

    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
        Agent = GetComponent<NavMeshAgent>();
        Agent.speed = MoveSpeed;
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        
    }

    private void OnEnable()
    {
        Controller = GetComponent<CharacterController>();
        Controller.enabled = true;

        Agent = GetComponent<NavMeshAgent>();
        
        Agent.speed = MoveSpeed;
        Agent.updatePosition = false;
        Agent.updateRotation = false;
        Agent.enabled = false;
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player").transform;

        Movement = new EnemyMovement(Controller, this, Agent);
        Health = new EnemyHealth(this);
        Health.CurrentHealth = MaxHealth;
        transform.position = new Vector3(transform.position.x, 50f, transform.position.z);
        StartCoroutine(SuperLanding());
    }
    private IEnumerator SuperLanding()
    {

        Animator.SetBool("IsJumping", true);
        float t = 0;
        yield return null;
        while (!Movement.CheckGrounded())
        {
            Movement.ApplyGravity();
            t += Time.deltaTime;
            if (t > 10)
            {
                ForceInit();
                yield break;
            }
            yield return null;
        }
        StateMachine = new EnemyStateMachine(this);
        switch (Type)
        {
            case EnemyType.Normal:
                StateMachine.ChangeState(new IdleState(this));
                break;
            case EnemyType.Follow:
                StateMachine.ChangeState(new FollowState(this));
                break;
            case EnemyType.Flee:
                StateMachine.ChangeState(new FleeState(this));
                break;
            case EnemyType.Elite_A:
                StateMachine.ChangeState(new EliteIdleState(this));
                break;
            case EnemyType.Elite_B:
                StateMachine.ChangeState(new EliteIdleState(this));
                break;
        }
        Movement.CheckGrounded();

        StartPosition = transform.position;
        Animator.SetBool("IsJumping", false);
        Agent.enabled = true;
        Agent.updatePosition = true;
        Agent.updateRotation = true;
    }

    public void ForceInit()
    {
        StateMachine = new EnemyStateMachine(this);
        switch (Type)
        {
            case EnemyType.Normal:
                StateMachine.ChangeState(new IdleState(this));
                break;
            case EnemyType.Follow:
                StateMachine.ChangeState(new FollowState(this));
                break;
            case EnemyType.Flee:
                StateMachine.ChangeState(new FleeState(this));
                break;
            case EnemyType.Elite_A:
                StateMachine.ChangeState(new EliteIdleState(this));
                break;
            case EnemyType.Elite_B:
                StateMachine.ChangeState(new EliteIdleState(this));
                break;
        }
        Movement.CheckGrounded();

        StartPosition = transform.position;
        Animator.SetBool("IsJumping", false);
        Agent.enabled = true;
        Agent.updatePosition = true;
        Agent.updateRotation = true;
    }

    private void Update()
    {
        if (Health.IsDead|| !Controller.enabled) return;
        //Animator.SetBool("IsGround", Controller.isGrounded);

        if (Agent.isOnOffMeshLink)
        {
            Animator.SetBool("IsJumping", true);  // 점프 애니메이션 트리거
        }
        else
        {
            Animator.SetBool("IsJumping", false); // 걷기나 일반 상태로 복귀
        }

        if (StateMachine != null)
        {
            StateMachine.Tick();
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        StateMachine?.CurrentState?.Exit();
    }



    public void TakeDamage(Damage dmg)
    {
        Health.ApplyDamage(dmg);
    }

    public void ReturnToPool()
    {
        EnemyPool.Instance.ReturnEnemy(Type, gameObject);
    }

    public void Attack()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            AttackRange,
            enemyLayer.value
        );

        foreach (var hit in hits)
        {
            var dmgable = hit.GetComponent<IDamageable>();
            if (dmgable != null)
            {
                var dmg = new Damage
                {
                    amount = AttackDamage,
                    type = DamageType.Explosion,
                    origin = transform.position,
                    knockbackForce = 0
                };
                dmgable.TakeDamage(dmg);
            }
        }
    }

    public void PerformAreaDamage(Vector3 origin, Vector3 forward, float angle, float radius)
    {
        Collider[] hits = Physics.OverlapSphere(origin, radius, enemyLayer);
        foreach (var hit in hits)
        {
            Vector3 toTarget = (hit.transform.position - origin).normalized;
            if (Vector3.Angle(forward, toTarget) <= angle * 0.5f)
            {
                if (hit.TryGetComponent(out IDamageable target))
                {
                    target.TakeDamage(new Damage
                    {
                        amount = AttackDamage,
                        origin = origin,
                        type = DamageType.Normal
                    });
                }
            }
        }
    }
}