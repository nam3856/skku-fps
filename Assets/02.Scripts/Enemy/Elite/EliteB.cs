using UnityEngine;

public class EliteB : MonoBehaviour
{
    public enum AttackType
    {
        Attack,
        AreaAttack,
    }
    public GameObject TrailRendererObj;
    public GameObject Indicator;
    public AudioSource AudioSource;
    public AudioClip[] AudioClips;
    public GameObject[] ShotEffects;
    public EnemyController EnemyController;
    public bool IsAttacking;
    public float[] AttackPosition;
    public float[] AttackDamage;
    public int AttackStep;
    public float[] AttackAngles;
    public float AttackRadius;
    public Vector3 Origin;


    public CharacterController TestController;
    public LayerMask TestLayer;
    public Animator TestAnimator;
    internal void StartAreaAttackRectIndicator()
    {
        IsAttacking = true;
        EnemyController.Agent.ResetPath();
        EnemyController.Agent.enabled = false;
        TrailRendererObj.SetActive(true);
        Indicator.SetActive(true);
        Indicator.GetComponent<RectAreaIndicator>().Length = 5f; 
        Indicator.GetComponent<RectAreaIndicator>().Width = 2f;
        Indicator.GetComponent<RectAreaIndicator>().index = 1;
    }

    internal void StartDashAttackRectIndicator()
    {
        Origin = transform.position;
        IsAttacking = true;
        TrailRendererObj.SetActive(true);
        Indicator.SetActive(true);

        Indicator.GetComponent<RectAreaIndicator>().Width = 2f;
        Indicator.GetComponent<RectAreaIndicator>().Length = 7f;

        Indicator.GetComponent<RectAreaIndicator>().index = 0;
    }
    
    internal void StartAttack(int index)
    {
        AttackType type = (AttackType)index;
        AttackStep = 0;
        EnemyController.Animator.applyRootMotion = true;
        //TestAnimator.applyRootMotion = true;
        //TestAnimator.SetTrigger(type.ToString());
        EnemyController.Animator.SetTrigger(type.ToString());
    }

    public void EndAttack()
    {
        EnemyController.Animator.applyRootMotion = false;
        //TestAnimator.applyRootMotion = false;
        EnemyController.Animator.SetTrigger("EndAttack");

        EnemyController.Agent.enabled = true;
        IsAttacking = false;
    }


    public void AreaAttack()
    {
        PerformAreaDamage(transform.position, transform.forward, AttackAngles[AttackStep], AttackRadius);
        AttackStep++;
    }

    public void DashAttack()
    {
        PerformRectDamage(2f, 7f);
    }

    private void PerformRectDamage(float width, float length, float height = 2f)
    {
        Vector3 center = Origin + transform.forward * (length / 2f);
        Vector3 halfExtents = new Vector3(width / 2f, height / 2f, length / 2f);
        Quaternion rotation = Quaternion.LookRotation(transform.forward);

        // 플레이어 + 장애물 레이어 마스크
        LayerMask mask = LayerMask.GetMask("Player", "Obstacle");

        Collider[] hits = Physics.OverlapBox(center, halfExtents, rotation, EnemyController.enemyLayer);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable dmgable))
            {
                dmgable.TakeDamage(new Damage
                {
                    amount = 4,
                    origin = transform.position,
                    type = DamageType.Normal
                });
            }
        }

        DebugDrawBox(center, halfExtents, rotation, Color.red, 2f);
    }

    private void PerformAreaDamage(Vector3 origin, Vector3 forward, float angle, float radius)
    {
        //Collider[] hits = Physics.OverlapSphere(origin, radius, TestLayer);
        Collider[] hits = Physics.OverlapSphere(origin, radius, EnemyController.enemyLayer);
        foreach (var hit in hits)
        {
            Vector3 toTarget = (hit.transform.position - origin).normalized;
            if (Vector3.Angle(forward, toTarget) <= angle * 0.5f)
            {
                if (hit.TryGetComponent(out IDamageable target))
                {
                    target.TakeDamage(new Damage
                    {
                        amount = AttackDamage[AttackStep],
                        origin = origin,
                        type = DamageType.Normal
                    });
                }
            }
        }
    }

    void OnAnimatorMove()
    {
        //if (TestAnimator.applyRootMotion)
        //{
        //    Vector3 deltaPosition = TestAnimator.deltaPosition;
        //    TestController.Move(deltaPosition);
        //}
        if (EnemyController.Animator.applyRootMotion)
        {
            Vector3 deltaPosition = EnemyController.Animator.deltaPosition;
            EnemyController.Controller.Move(deltaPosition);
        }
    }


#if UNITY_EDITOR
    private void DebugDrawBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, Color color, float duration)
    {
        var points = new Vector3[8];

        Matrix4x4 m = Matrix4x4.TRS(center, orientation, Vector3.one);
        for (int i = 0; i < 8; i++)
        {
            Vector3 corner = new Vector3(
                (i & 1) == 0 ? -1 : 1,
                (i & 2) == 0 ? -1 : 1,
                (i & 4) == 0 ? -1 : 1
            );
            points[i] = m.MultiplyPoint3x4(Vector3.Scale(halfExtents, corner));
        }

        Debug.DrawLine(points[0], points[1], color, duration);
        Debug.DrawLine(points[1], points[3], color, duration);
        Debug.DrawLine(points[3], points[2], color, duration);
        Debug.DrawLine(points[2], points[0], color, duration);

        Debug.DrawLine(points[4], points[5], color, duration);
        Debug.DrawLine(points[5], points[7], color, duration);
        Debug.DrawLine(points[7], points[6], color, duration);
        Debug.DrawLine(points[6], points[4], color, duration);

        Debug.DrawLine(points[0], points[4], color, duration);
        Debug.DrawLine(points[1], points[5], color, duration);
        Debug.DrawLine(points[2], points[6], color, duration);
        Debug.DrawLine(points[3], points[7], color, duration);
    }
#endif
}
