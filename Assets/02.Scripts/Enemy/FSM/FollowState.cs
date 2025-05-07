using UnityEngine;

public class FollowState : IEnemyState
{
    private readonly EnemyController _enemyController;

    public FollowState(EnemyController ctx)
    {
        _enemyController = ctx;
    }

    public void Enter()
    {
        _enemyController.Animator.SetBool("IsMoving", true);
        _enemyController.Animator.SetBool("IsSprinting", true);
        _enemyController.UI_Enemy.SetStateText("TRACING");
    }

    public void Execute()
    {
        if (Vector3.Distance(_enemyController.transform.position, _enemyController.Player.position) < _enemyController.AttackDistance)
        {
            Debug.Log($"{_enemyController.gameObject.name}: Follow -> Attack");
            _enemyController.StateMachine.ChangeState(new AttackState(_enemyController));
            return;
        }
        _enemyController.Animator.SetBool("IsMoving", true);
        _enemyController.Animator.SetBool("IsSprinting", true);
        Vector3 target = _enemyController.Player.position;
        _enemyController.Movement.Move(target, _enemyController.MoveSpeed);

        
    }

    public void Exit()
    {
        _enemyController.Animator.SetBool("IsSprinting", false);
    }
}
