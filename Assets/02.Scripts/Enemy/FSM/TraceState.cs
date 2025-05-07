using UnityEngine;

public class TraceState : IEnemyState
{
    private readonly EnemyController _enemyController;

    public TraceState(EnemyController ctx) { _enemyController = ctx; }

    public void Enter() {

        _enemyController.UI_Enemy.SetStateText("TRACING");
        _enemyController.Agent.speed = _enemyController.MoveSpeed;

        
    }

    public void Execute()
    {
        _enemyController.Animator.SetBool("IsMoving", true);
        float dist = Vector3.Distance(_enemyController.transform.position, _enemyController.Player.position);

        if (dist < _enemyController.AttackDistance)
        {
            _enemyController.StateMachine.ChangeState(new AttackState(_enemyController));
            return;
        }
        if (dist > _enemyController.FindDistance + _enemyController.AggressiveTraceDistance)
        {
            _enemyController.StateMachine.ChangeState(new ReturnState(_enemyController));
            return;
        }

        _enemyController.Movement.Move(_enemyController.Player.position, _enemyController.MoveSpeed);
    }

    public void Exit() { }
}