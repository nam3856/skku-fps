using UnityEngine;
using UnityEngine.AI;

public class FleeState : IEnemyState
{
    private EnemyController _enemyController;
    private float safeDistance = 10f;

    public FleeState(EnemyController c) { _enemyController = c; }

    public void Enter() {

        _enemyController.Animator.SetBool("IsMoving", true);
        _enemyController.UI_Enemy.SetStateText("FLEEING");
    }

    public void Execute()
    {
        Vector3 enemyPos = _enemyController.transform.position;
        Vector3 playerPos = _enemyController.Player.position;
        float horizontalDist = Vector3.Distance(
            new Vector3(enemyPos.x, 0, enemyPos.z),
            new Vector3(playerPos.x, 0, playerPos.z)
        );

        if (horizontalDist > safeDistance)
        {
            Debug.Log("Flee -> Idle");
            _enemyController.StateMachine.ChangeState(new IdleState(_enemyController));
            return;
        }

        Vector3 dir = (enemyPos - playerPos);
        dir.y = 0;
        dir.Normalize();

        Vector3 rawTarget = enemyPos + dir * safeDistance;

        NavMeshHit hit;
        Vector3 fleeTarget;
        if (NavMesh.SamplePosition(rawTarget, out hit, safeDistance, NavMesh.AllAreas))
            fleeTarget = hit.position;
        else
            fleeTarget = new Vector3(rawTarget.x, enemyPos.y, rawTarget.z);

        _enemyController.Movement.Move(fleeTarget, _enemyController.MoveSpeed);
    }

    public void Exit() { }
}
