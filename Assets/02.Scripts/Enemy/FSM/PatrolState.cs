using UnityEngine;
using UnityEngine.AI;

public class PatrolState : IEnemyState
{
    private readonly EnemyController _enemyController;
    private Vector3 _targetPoint;
    private int _index;

    public PatrolState(EnemyController ctx)
    {
        _enemyController = ctx;
    }

    public void Enter()
    {

        _enemyController.UI_Enemy.SetStateText("PATROL");
        NavMeshPath path = new NavMeshPath();
        Vector3 candidate;
        Vector3 pos = _enemyController.transform.position;
        pos.y = 0;
        for (int i = 0; i < 10; i++)
        {
            candidate = pos + new Vector3(
                Random.Range(_enemyController.MinPatrolRange, _enemyController.MaxPatrolRange),
                0,
                Random.Range(_enemyController.MinPatrolRange, _enemyController.MaxPatrolRange)
            );
            if (_enemyController.Agent.CalculatePath(candidate, path) &&
                path.status == NavMeshPathStatus.PathComplete)
            {
                _targetPoint = candidate;

                return;
            }
        }

        _targetPoint = _enemyController.StartPosition;
        _enemyController.Agent.speed = _enemyController.PatrolSpeed;

        _enemyController.UI_Enemy.SetStateText($"Failed to Find");
    }

    public void Execute()
    {
        _enemyController.Animator.SetBool("IsMoving", true);
        var distToPlayer = Vector3.Distance(_enemyController.transform.position, _enemyController.Player.position);
        if (distToPlayer < _enemyController.FindDistance)
        {
            if (_enemyController.Type == EnemyType.Flee)
            {
                Debug.Log($"{_enemyController.gameObject.name}: Patrol -> Flee");
                _enemyController.StateMachine.ChangeState(new FleeState(_enemyController));
                return;
            }
            Debug.Log($"{_enemyController.gameObject.name}: Patrol -> Trace");
            _enemyController.StateMachine.ChangeState(new TraceState(_enemyController));
            return;
        }
        Vector2 me = new Vector2(_enemyController.transform.position.x, _enemyController.transform.position.z);
        Vector2 tgt = new Vector2(_targetPoint.x, _targetPoint.z);
        var dist = Vector3.Distance(me, tgt);
        if (dist < 0.5f)
        {
            if (_index < _enemyController.PatrolCount)
            {
                _index++;
                Enter();
                return;
            }
            _enemyController.StateMachine.ChangeState(new ReturnState(_enemyController));
            return;
        }

        _enemyController.Movement.Move(_targetPoint, _enemyController.PatrolSpeed);
    }

    public void Exit() { }
}