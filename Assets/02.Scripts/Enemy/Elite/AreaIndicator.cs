using UnityEngine;

public class AreaIndicator : MonoBehaviour
{
    [Header("설정")]
    public float maxAngle = 90f;
    public float maxRadius = 5f;
    public float growDuration = 1.5f;

    [Header("완료 후 공격할 대상")]
    public EnemyController owner; // 또는 EnemyBase 등

    public EliteA EliteTest;

    private Material _mat;
    private float _timer;
    private bool _finished;

    private void Awake()
    {
        _mat = GetComponent<Renderer>().material;
        _mat.SetFloat("_Angle", 0f);
        _mat.SetFloat("_Radius", 0f);
    }

    private void OnEnable()
    {
        _mat.SetFloat("_Angle", 0f);
        _mat.SetFloat("_Radius", 0f);
    }

    private void Update()
    {
        if (_finished) return;

        _timer += Time.deltaTime;
        float t = Mathf.Clamp01(_timer / growDuration);

        float currentAngle = Mathf.Lerp(0f, maxAngle, t);
        float currentRadius = Mathf.Lerp(0f, maxRadius, t);

        _mat.SetFloat("_Angle", currentAngle);
        _mat.SetFloat("_Radius", currentRadius);

        if (t >= 1f)
        {
            _finished = true;
            OnIndicatorComplete();
        }
    }

    private void OnIndicatorComplete()
    {
        if (owner != null)
        {
            owner.Animator.SetTrigger("AreaAttack");
        }
        EliteTest.StartRotatingAreaAttack();

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _finished = false;
        _timer = 0;
    }
}
