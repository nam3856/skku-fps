using System.Collections;
using UnityEngine;

public class RectAreaIndicator : MonoBehaviour
{
    [Header("공격 정보")]
    public EliteB owner;
    public float growDuration = 1.5f;
    private float _width;
    public float Width
    {
        get
        {
            return _width;
        }
        set
        {
            _width = value;
            FinalRect.transform.localScale = new Vector3(value, 1f, _length);
        }
    }
    private float _length;
    public float Length
    {
        get
        {
            return _length;
        }
        set
        {
            _length = value;
            FinalRect.transform.localScale = new Vector3(_width, 1f, value);
        }
    }

    private float _timer;
    private Vector3 _targetScale;
    private bool _finished;
    public GameObject FinalRect;
    public int index;

    private void Awake()
    {
        transform.localScale = new Vector3(0f, 1f, 0f);
        _targetScale = new Vector3(Width, 1f, Length);
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

        float currentLength = Mathf.Lerp(0f, _length, t);

        transform.localScale = new Vector3(_width, 1f, currentLength);
        //transform.localPosition = transform.forward * (currentLength * 0.5f) + Offset;
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
