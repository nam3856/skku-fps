using System.Collections;
using UnityEngine;



public class EliteA : MonoBehaviour
{
    public Animator Animator;
    public Transform Pivot;
    public GameObject Indicator;
    public Vector3 OriginalRotation;
    public AudioSource AudioSource;
    public AudioClip[] AudioClips;
    public GameObject[] ShotEffects;
    public EnemyController EnemyController;
    public bool IsAttacking;

    
    public void StartAreaAttack()
    {

        IsAttacking = true;
        //var indicator = Instantiate(areaIndicatorPrefab, transform.position + Vector3.up * 0.01f, transform.rotation);
        Indicator.SetActive(true);
        Indicator.GetComponent<AreaIndicator>().EliteTest = this;
    }

    public void StartRotatingAreaAttack()
    {
        EnemyController.Animator.SetTrigger("AreaAttack");
        StartCoroutine(RotateOverTime(1.5f));
        foreach (var e in ShotEffects)
        {
            e.SetActive(true);
        }
    }
    private IEnumerator RotateOverTime(float duration)
    {
        float timer = 0f;
        float targetAngle = 90f;
        float speed = targetAngle / duration;
        OriginalRotation = transform.localEulerAngles;
        transform.Rotate(0f, -speed*duration/2, 0f);
        EnemyController.Agent.updateRotation = false;
        while (timer < duration)
        {
            float delta = speed * Time.deltaTime;
            transform.Rotate(0f, delta, 0f);
            timer += Time.deltaTime;
            if ((int)(timer * 100) % 5 == 0)
            {
                AudioSource.PlayOneShot(AudioClips[Random.Range(0,AudioClips.Length)]);
                PerformAreaDamage(Pivot.position, Pivot.forward, 90f, 5f);
            }
            yield return null;
        }
        transform.localEulerAngles = OriginalRotation;
        EnemyController.Animator.SetTrigger("AreaAttackEnd");
        foreach (var e in ShotEffects)
        {
            e.SetActive(false);
        }
        IsAttacking = false;
        EnemyController.Agent.updateRotation = true;
    }

    public void PerformAreaDamage(Vector3 origin, Vector3 forward, float angle, float radius)
    {
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
                        amount = 1,
                        origin = origin,
                        type = DamageType.Normal
                    });
                }
            }
        }
    }
}
