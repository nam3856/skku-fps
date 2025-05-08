using System.Collections;
using UnityEngine;

public class RectAreaIndicator : MonoBehaviour
{
    [Header("공격 정보")]
    public EliteB owner;
    public float growDuration = 1.5f;
    public float width = 4f;
    public float length = 6f;

    private float _timer;
    private Vector3 _targetScale;
    private bool _finished;
    public Vector3 Offset = new Vector3(0, 0.2040305f, 0);
    public GameObject FinalRect;
    public int index;

    private void Awake()
    {
        transform.localScale = new Vector3(0f, 1f, 0f);
        _targetScale = new Vector3(width, 1f, length);
    }
    private void OnEnable()
    {
        transform.localScale = new Vector3(0f, 1f, 0f);
        FinalRect.SetActive(true);
    }
    private void Update()
    {
        if (_finished) return;

        _timer += Time.deltaTime;
        float t = Mathf.Clamp01(_timer / growDuration);

        float currentWidth = Mathf.Lerp(0f, width, t);
        float currentLength = Mathf.Lerp(0f, length, t);

        transform.localScale = new Vector3(currentWidth, 1f, currentLength);

        transform.localPosition = -transform.forward * currentLength * 4 + Offset;
        if (t >= 1f)
        {
            _finished = true;
            OnComplete();
        }
    }

    private void OnComplete()
    {
        if (owner != null)
        {
            owner.StartAttack(index);
        }
        gameObject.SetActive(false);
        FinalRect.SetActive(false);
    }

    private void OnDisable()
    {
        _finished = false;
        _timer = 0;
    }
}
