using Unity.VisualScripting;
using UnityEngine;

public class DieState : IEnemyState
{
    private readonly EnemyController _enemyController;
    private bool _isExplodeDeath;
    public DieState(EnemyController ctx, bool explode)
    {
        _enemyController = ctx;
        _isExplodeDeath = explode;
    }

    public void Enter()
    {
        _enemyController.UI_Enemy.SetStateText("");
        if (_isExplodeDeath || !_enemyController.Controller.isGrounded)
        {
            _enemyController.Animator.SetInteger("RAND", Random.Range(2, 4));
        }
        else
        {
            _enemyController.Animator.SetInteger("RAND", Random.Range(0, 2));
        }

        _enemyController.Animator.SetTrigger("Die");
        _enemyController.Movement.Move(Vector3.zero, 0);
        _enemyController.GetComponent<CharacterController>().enabled = false;
        if (_enemyController.DeathEffect)
        {
            GameObject.Instantiate(_enemyController.DeathEffect, _enemyController.transform.position, Quaternion.identity);
        }
        _enemyController.Invoke(nameof(_enemyController.ReturnToPool), 3f);
    }

    public void Execute() {
        _enemyController.Movement.CheckGrounded();
        _enemyController.Movement.ApplyGravity();
    }
    public void Exit() { }
}