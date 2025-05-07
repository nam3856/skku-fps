using UnityEngine;

public class EnemyHealth
{
    private readonly EnemyController _enemyController;
    public float MaxHealth => _enemyController.MaxHealth;
    public float CurrentHealth { get; set; }
    public bool IsDead { get; private set; }
    private float _lastDamageTime;

    public EnemyHealth(EnemyController ctx)
    {
        _enemyController = ctx;

        _enemyController.UI_Enemy.SetHealthFill(1);
    }

    public void ApplyDamage(Damage dmg)
    {
        if (IsDead) return;

        CurrentHealth -= dmg.amount;
        _lastDamageTime = Time.time;
        _enemyController.HitEffect?.Play();

        _enemyController.UI_Enemy.SetHealthFill(CurrentHealth / MaxHealth);

        if (CurrentHealth <= 0)
        {
            IsDead = true;
            _enemyController.StateMachine.OnDead(dmg);
        }
        else
        {
            if(_enemyController.Type==EnemyType.Elite_A || _enemyController.Type == EnemyType.Elite_B)
            {
                return;
            }
            else
            {
                _enemyController.Animator.SetInteger("RAND", Random.Range(1, 3));
                _enemyController.Animator.SetTrigger("HIT");
                if (_enemyController.Controller != null)
                {
                    if (_enemyController.StateMachine != null)
                    {
                        _enemyController.StateMachine.OnDamaged(dmg);
                    }
                    else
                    {
                        _enemyController.ForceInit();
                        _enemyController.StateMachine.OnDamaged(dmg);
                    }
                }
            }
                
        }
    }
}