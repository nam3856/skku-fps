using UnityEngine;

public class EnemyAttackEvent : MonoBehaviour
{
    public EnemyController MyEnemy;

    public void AttackEvent()
    {
        MyEnemy.Attack();
    }

    public void AreaAttackEvent()
    {
        MyEnemy.PerformAreaDamage(transform.position, transform.forward, 90f, 5f); // angle, radius
    }

}
