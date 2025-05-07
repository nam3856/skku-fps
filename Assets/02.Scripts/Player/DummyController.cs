using UnityEngine;
using System.Collections;

public class DummyController : MonoBehaviour
{
    [SerializeField] private PlayerMove _playerMove;
    [SerializeField] private GameObject[] _toHide;

    private bool _prevHideState;
    private Coroutine _delayCoroutine;

    private void Update()
    {
        bool nowHide = _playerMove.HideDummy;

        if (nowHide != _prevHideState)
        {
            if (_delayCoroutine != null)
            {
                StopCoroutine(_delayCoroutine);
                _delayCoroutine = null;
            }

            if (!nowHide)
                _delayCoroutine = StartCoroutine(ShowAfterDelay(0.34f));
           
            else
                _delayCoroutine = StartCoroutine(HideAfterDelay(0.04f));
        }

        _prevHideState = nowHide;
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetAllActive(false);
        _delayCoroutine = null;
    }

    private IEnumerator ShowAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetAllActive(true);
        _delayCoroutine = null;
    }

    private void SetAllActive(bool isActive)
    {
        foreach (var obj in _toHide)
            obj.SetActive(isActive);
    }
}
