using UnityEngine;

public class EliteIdleState : IEnemyState
{
    private readonly EnemyController _enemyController;
    private float _timer;

    public EliteIdleState(EnemyController ctx)
    {
        _enemyController = ctx;
    }

    public void Enter()
    {
        _timer = 0f;
        if (_enemyController.Agent.hasPath)
            _enemyController.Agent.ResetPath();
        _enemyController.Animator.SetBool("IsMoving", false);

        _enemyController.UI_Enemy.SetStateText("IDLE");
    }

    public void Execute()
    {
        var dist = Vector3.Distance(_enemyController.transform.position, _enemyController.Player.position);
        if (dist < _enemyController.FindDistance)
        {
            _enemyController.StateMachine.ChangeState(new TraceState(_enemyController));
            return;
        }
        _enemyController.Animator.SetBool("IsMoving", false);
        _timer += Time.deltaTime;
        if (_timer > _enemyController.PatrolThreshold)
        {
            _enemyController.StateMachine.ChangeState(new TraceState(_enemyController));
        }

    }

    public void Exit() { }
}