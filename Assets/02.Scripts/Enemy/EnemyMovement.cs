using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement
{
    private readonly CharacterController _characterController;
    private readonly NavMeshAgent _agent;
    private readonly EnemyController _enemyController;
    private Vector3 _verticalVelocity;
    private float _verticalSpeed;
    private float groundCheckDistance = 0.5f;
    


    public bool IsGrounded => _characterController.isGrounded;

    public EnemyMovement(CharacterController cc, EnemyController ctx, NavMeshAgent agent)
    {
        _characterController = cc;
        _enemyController = ctx;
        _agent = agent;
        _agent.updatePosition = true;
        _agent.updateRotation = true;
    }

    public bool CheckGrounded()
    {
        bool onGround = Physics.Raycast(_enemyController.transform.position, Vector3.down, groundCheckDistance, _enemyController.GroundLayer);
        if (onGround)
        {
            _verticalSpeed = -2f;
        }
        else
        {
            _verticalSpeed -= EnemyController.Gravity * Time.deltaTime;
        }
        _verticalVelocity = Vector3.up * _verticalSpeed;

        return onGround;
    }

    public void ApplyGravity()
    {
        if (!_enemyController.Controller.enabled) return;
        _characterController.Move(_verticalVelocity * Time.deltaTime);
    }

    public void Move(Vector3 targetPosition, float speed)
    {
        if (!_characterController.enabled) return;

        if(targetPosition == Vector3.zero)
        {
            _agent.ResetPath();
            return;
        }
        if (!_agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(_enemyController.transform.position, out hit, 2f, NavMesh.AllAreas))
                _agent.Warp(hit.position);
            else
                return;
        }

        _agent.speed = speed;
        _agent.SetDestination(targetPosition);

        bool isMoving = _agent.velocity.sqrMagnitude > 0.01f;
        _enemyController.Animator.SetBool("IsMoving", isMoving);
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _characterController.Move(direction * force * Time.deltaTime);
    }

    public void ResumeNavMeshAgent()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(_enemyController.transform.position, out hit, 1f, NavMesh.AllAreas))
            _agent.Warp(hit.position);

        _agent.updatePosition = true;
        _agent.updateRotation = true;
    }
}