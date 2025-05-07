using UnityEngine;
using UnityEngine.AI;

public class DamagedState : IEnemyState
{
    private readonly EnemyController _enemyController;
    private readonly Damage _damage;
    private readonly float _endTime;
    private readonly float _duration;
    public DamagedState(EnemyController ctx, Damage damage)
    {
        _enemyController = ctx;
        _damage = damage;
        _endTime = Time.time + _enemyController.KnockbackDuration;
        _duration = Time.time + _enemyController.StunDuration;
    }

    public void Enter()
    {
        _enemyController.Animator.SetInteger("RAND", Random.Range(0, 2));
        _enemyController.Animator.SetTrigger("HIT");
    }

    public void Execute()
    {
        if (Time.time < _endTime)
        {
            Vector3 dir = (_enemyController.transform.position - _damage.origin).normalized;
            _enemyController.Movement.ApplyKnockback(dir, _damage.knockbackForce);

            _enemyController.Movement.CheckGrounded();
            _enemyController.Movement.ApplyGravity();
            return;
        }
        _enemyController.Movement.CheckGrounded();
        _enemyController.Movement.ApplyGravity();
        if (Time.time < _duration) return;

        _enemyController.Movement.ResumeNavMeshAgent();
        _enemyController.StateMachine.ChangeState(new TraceState(_enemyController));
    }

    public void Exit() { }
}