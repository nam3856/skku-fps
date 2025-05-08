using System;
using UnityEngine;

public class AttackState : IEnemyState
{
    private readonly EnemyController _context;
    private float _lastAttack;
    private float _lastSkill;
    public GameObject areaIndicatorPrefab;
    public AttackState(EnemyController ctx) { _context = ctx; }
    public void Enter()
    {
        _context.UI_Enemy.SetStateText("ATTACK");
        _context.Animator.SetBool("IsMoving", false);
        _context.Agent.ResetPath();
        _lastAttack = _context.LastAttack;
        _lastSkill = _context.LastSkill;
    }

    public void Execute()
    {
        switch (_context.Type)
        {
            case EnemyType.Elite_A:
                if (Time.time >= _lastSkill + _context.SkillCooldown)
                {
                    PerformAreaAttack();
                }
                else
                {
                    //PerformSingleAttack();
                }
                break;
            case EnemyType.Elite_B:
                if (Time.time >= _lastSkill + _context.SkillCooldown)
                {
                    PerformRectAreaAttack();
                }
                else
                {
                    PerformDashAttack();
                }
                break;

            default:
                PerformSingleAttack();
                break;
        }
    }

    private void PerformRectAreaAttack()
    {
        if (_context.EliteBScript.IsAttacking) return;
        if (CheckDistance() && !_context.EliteBScript.IsAttacking)
        {
            StateChange();
            return;
        }
        _context.transform.LookAt(new Vector3(_context.Player.position.x, _context.transform.position.y, _context.Player.position.z));
        _context.EliteBScript.StartAreaAttackRectIndicator();
        _lastSkill = Time.time;
    }

    private void PerformDashAttack()
    {
        if (_context.EliteBScript.IsAttacking) return;
        if (CheckDistance() && !_context.EliteBScript.IsAttacking)
        {
            StateChange();
            return;
        }
        if (Time.time >= _lastAttack + _context.AttackCooldown) 
        {
            _context.transform.LookAt(new Vector3(_context.Player.position.x, _context.transform.position.y, _context.Player.position.z));
            _context.EliteBScript.StartDashAttackRectIndicator();
            _lastAttack = Time.time;
        }
    }

    private void PerformAreaAttack()
    {
        if (CheckDistance() && !_context.EliteAScript.IsAttacking)
        {
            StateChange();
            return;
        }

        _context.transform.LookAt(new Vector3(_context.Player.position.x, _context.transform.position.y, _context.Player.position.z));

        _context.EliteAScript.StartAreaAttack();

        _lastSkill = Time.time;
    }

    private void PerformSingleAttack()
    {
        if (CheckDistance())
        {
            StateChange();
            return;
        }
        _context.transform.LookAt(new Vector3(_context.Player.position.x, _context.transform.position.y, _context.Player.position.z));

        if (Time.time >= _lastAttack + _context.AttackCooldown)
        {
            _context.Animator.SetTrigger("Attack");

            _lastAttack = Time.time;
        }
    }

    private bool CheckDistance()
    {
        float dist = Vector3.Distance(_context.transform.position, _context.Player.position);
        if (dist > _context.AttackDistance)
        {
            return true;
        }
        return false;
    }

    private void StateChange()
    {
        _context.StateMachine.ChangeState(
                      _context.Type == EnemyType.Follow
                        ? (IEnemyState)new FollowState(_context)
                        : new TraceState(_context)
                        );
    }

    public void Exit()
    {
        _context.LastAttack = _lastAttack;
        _context.LastSkill = _lastSkill;
    }
}