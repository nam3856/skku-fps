public class EnemyStateMachine
{
    private readonly EnemyController _context;
    public IEnemyState CurrentState;

    public EnemyStateMachine(EnemyController ctx)
    {
        _context = ctx;
    }

    public void Initialize()
    {
        ChangeState(new IdleState(_context));
    }

    public void Tick()
    {
        CurrentState.Execute();
    }

    public void ChangeState(IEnemyState newState)
    {
        if (CurrentState == newState) return;
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void OnDamaged(Damage damage)
    {
        ChangeState(new DamagedState(_context, damage));
    }

    public void OnDead(Damage damage)
    {
        ChangeState(new DieState(_context, damage.type==DamageType.Explosion));
    }
}