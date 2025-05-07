using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    // 공통 구성 요소들
    public EnemyHealth Health { get; protected set; }
    public EnemyMovement Movement { get; protected set; }
    public EnemyUI UI_Enemy { get; protected set; }
    public Animator Animator { get; protected set; }
    public Transform Player { get; protected set; }
    public CharacterController Controller { get; protected set; }
    public NavMeshAgent Agent { get; protected set; }

    public LayerMask GroundLayer;
    public Vector3 StartPosition { get; protected set; }
    public EnemyStateMachine StateMachine { get; protected set; }

    public float MaxHealth { get; private set; }

    // 공통 메서드들
    public abstract void TakeDamage(Damage dmg);
    public abstract void ReturnToPool();
    public abstract void Attack();
    public abstract void Initialize();
    public abstract void Update();
}
