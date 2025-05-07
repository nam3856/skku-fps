using UnityEngine;

public class ReturnState : IEnemyState
{
    private readonly EnemyController _enemyController;

    public ReturnState(EnemyController ctx) { _enemyController = ctx; }

    public void Enter()
    {
        _enemyController.UI_Enemy.SetStateText("RETURN");
        _enemyController.Animator.SetBool("IsMoving", true);
    }

    public void Execute()
    {
        _enemyController.Animator.SetBool("IsMoving", true);

        Vector2 me = new Vector2(_enemyController.transform.position.x, _enemyController.transform.position.z);
        Vector2 tgt = new Vector2(_enemyController.StartPosition.x, _enemyController.StartPosition.z);
        var distHome = Vector3.Distance(me, tgt);

        var distToPlayer = Vector3.Distance(_enemyController.transform.position, _enemyController.Player.position);
        if (distToPlayer < _enemyController.FindDistance)
        {
            if (_enemyController.Type == EnemyType.Flee)
            {
                _enemyController.StateMachine.ChangeState(new FleeState(_enemyController));
                return;
            }
            _enemyController.StateMachine.ChangeState(new TraceState(_enemyController));
            return;
        }

        if (distHome < _enemyController.ReturnDistance)
        {
            if (_enemyController.Type == EnemyType.Elite_A || _enemyController.Type == EnemyType.Elite_B)
            {
                _enemyController.StateMachine.ChangeState(new EliteIdleState(_enemyController)); 
                return;
            }
            else
            {
                _enemyController.StateMachine.ChangeState(new IdleState(_enemyController)); 
                return;
            }
        }
        _enemyController.Movement.Move(_enemyController.StartPosition, _enemyController.MoveSpeed);
    }

    public void Exit() { }
}