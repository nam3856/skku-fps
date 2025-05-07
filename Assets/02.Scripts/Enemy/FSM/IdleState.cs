using UnityEngine;

public class IdleState : IEnemyState
{
    private readonly EnemyController _enemyController;
    private float _timer;

    public IdleState(EnemyController ctx)
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
            if(_enemyController.Type == EnemyType.Flee)
            {
                Debug.Log($"{_enemyController.gameObject.name}: Idle -> Flee");
                _enemyController.StateMachine.ChangeState(new FleeState(_enemyController));
                return;
            }

            Debug.Log($"{_enemyController.gameObject.name}: Idle -> Trace");
            _enemyController.StateMachine.ChangeState(new TraceState(_enemyController));
            return;
        }
        _enemyController.Animator.SetBool("IsMoving", false);
        _timer += Time.deltaTime;
        if (_timer > _enemyController.PatrolThreshold)
        {
            Debug.Log($"{_enemyController.gameObject.name}: Idle -> Patrol");
            _enemyController.StateMachine.ChangeState(new PatrolState(_enemyController));
        }
            
    }

    public void Exit() { }
}