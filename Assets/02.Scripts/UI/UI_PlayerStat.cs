using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public struct WeaponData
{
    public int MaxAmmo;
    public int MaxGranade;
}

public class UI_PlayerStat : MonoBehaviour
{
    #region Singleton
    public static UI_PlayerStat Instance { get; private set; }
    #endregion

    #region References
    [Header("UI References")]
    [SerializeField] private RectTransform _steminaFillRect;
    [SerializeField] private Image _healthFillRect;
    [SerializeField] private TextMeshProUGUI _ammoText;
    [SerializeField] private TextMeshProUGUI _granadeText;
    [SerializeField] private Image _cancelImage;
    [SerializeField] private Image _reloadSpinner;
    [SerializeField] private Image[] _reloadCancelledSpinners;
    [SerializeField] private Image[] _reloadSuccessImages;
    [SerializeField] private Image _hitEffectImage;
    [SerializeField] private GameObject[] _weaponImages; 
    #endregion

    #region Settings
    [Header("Settings")]
    [SerializeField] private float _maxWidth = 500f;
    [SerializeField] private float _fillSpeed = 10f;
    [SerializeField] private float _endRotation = -720f;
    [SerializeField] private Vector2[] _offsets;
    [SerializeField] private float HitEffectDuration;
    [SerializeField] private float _endAlphaThreshold;
    #endregion

    #region Private Fields
    private Vector3[] _spinnersOrigin;
    private int _maxAmmo;
    private int _maxGranade;
    private bool _reloading;
    private float _lastHealthRatio = 1;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeSpinners();
    }
    #endregion

    #region Public Methods
    public void Init(WeaponData data)
    {
        _maxAmmo = data.MaxAmmo;
        _maxGranade = data.MaxGranade;
        SetAmmo(_maxAmmo);
        SetGranade(_maxGranade);
    }
    private Coroutine coroutine;
    public void SetHealth(float healthRatio)
    {
        if(_lastHealthRatio > healthRatio)
        {
            _hitEffectImage.color = Color.white;
            if(coroutine != null)
                StopCoroutine(coroutine);
            coroutine = StartCoroutine(HideAfterDuration());
        }
        _lastHealthRatio = healthRatio;
        _healthFillRect.fillAmount = healthRatio;
    }
    private IEnumerator HideAfterDuration()
    {
        float elapsed = 0f;
        Color col = _hitEffectImage.color;
        float startAlpha = col.a;

        float k = -Mathf.Log(_endAlphaThreshold / startAlpha) / HitEffectDuration;

        while (elapsed < HitEffectDuration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = startAlpha * Mathf.Exp(-k * elapsed);
            col.a = newAlpha;
            _hitEffectImage.color = col;
            yield return null;
        }

        col.a = 0f;
        _hitEffectImage.color = col;
    }
    public void SetStamina(float staminaRatio)
    {
        var size = _steminaFillRect.sizeDelta;
        size.x = _maxWidth * Mathf.Clamp01(staminaRatio);
        _steminaFillRect.sizeDelta = Vector2.Lerp(_steminaFillRect.sizeDelta, 
            new Vector2(_maxWidth * staminaRatio, _steminaFillRect.sizeDelta.y), 
            Time.deltaTime * 10f);
    }

    public void SetAmmo(int ammo)
    {
        _ammoText.text = $"{ammo}/{_maxAmmo}";
    }

    public void SetGranade(int granade)
    {
        _granadeText.text = $"{granade}/{_maxGranade}";
    }
    #endregion

    #region Reload Methods
    public IEnumerator StartReload(float reloadTime)
    {
        if (_reloading) yield break;
        _reloading = true;

        ResetReloadUI();
        HideCancelUI();
        yield return StartCoroutine(AnimateReloadSpinner(reloadTime));
        if (!_reloading) yield break;

        yield return StartCoroutine(AnimateSuccessImages());
        yield return StartCoroutine(FadeOutReloadSpinner());

        ResetReloadSuccessUI();
        _reloading = false;
    }

    public IEnumerator CancelReload()
    {
        if (!_reloading) yield break;
        _reloading = false;

        _reloadSpinner.color = Color.clear;
        ShowCancelUI();
        yield return StartCoroutine(AnimateCancelSpinners());
        yield return StartCoroutine(FadeOutCancelUI());
    }
    #endregion

    #region Private Methods
    private void InitializeSpinners()
    {
        _spinnersOrigin = new Vector3[_reloadCancelledSpinners.Length];
        for (int i = 0; i < _reloadCancelledSpinners.Length; i++)
        {
            _spinnersOrigin[i] = _reloadCancelledSpinners[i].GetComponent<RectTransform>().anchoredPosition;
        }
    }

    private void ResetReloadUI()
    {
        foreach (var spinner in _reloadCancelledSpinners)
        {
            spinner.color = new Color(0, 0, 0, 0);
        }
        _cancelImage.color = new Color(1, 0, 0, 0);
        _reloadSpinner.GetComponent<RectTransform>().eulerAngles = Vector3.zero;
        _reloadSpinner.color = Color.white;
    }

    private IEnumerator AnimateReloadSpinner(float reloadTime)
    {
        float t = 0;
        while (t < reloadTime)
        {
            float time = t / reloadTime;
            float zRotation = Mathf.Lerp(0, _endRotation, time);
            _reloadSpinner.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, zRotation);
            t += Time.deltaTime;
            if (!_reloading) yield break;
            yield return null;
        }
        _reloadSpinner.GetComponent<RectTransform>().eulerAngles = Vector3.zero;
    }

    private IEnumerator AnimateSuccessImages()
    {
        foreach (var image in _reloadSuccessImages)
        {
            while (image.fillAmount < 1)
            {
                image.fillAmount = Mathf.Clamp01(image.fillAmount + Time.deltaTime * _fillSpeed);
                yield return null;
            }
        }
    }

    private IEnumerator FadeOutReloadSpinner()
    {
        float t = 0;
        while (t < 0.3f)
        {
            _reloadSpinner.color = new Color(1, 1, 1, (0.3f - t) * 3);
            t += Time.deltaTime;
            yield return null;
        }
        _reloadSpinner.color = Color.clear;
    }

    private void ResetReloadSuccessUI()
    {
        foreach (var image in _reloadSuccessImages)
        {
            image.fillAmount = 0;
        }
    }

    private void ShowCancelUI()
    {
        _cancelImage.color = new Color(1, 0, 0, 1);
        for (int i = 0; i < _reloadCancelledSpinners.Length; i++)
        {
            _reloadCancelledSpinners[i].color = new Color(1, 1, 1, 1);
            _reloadCancelledSpinners[i].GetComponent<RectTransform>().anchoredPosition = _spinnersOrigin[i];
        }
    }
    private void HideCancelUI()
    {
        _cancelImage.color = new Color(1, 0, 0, 0);
        for (int i = 0; i < _reloadCancelledSpinners.Length; i++)
        {
            _reloadCancelledSpinners[i].color = new Color(1, 1, 1, 0);
            _reloadCancelledSpinners[i].GetComponent<RectTransform>().anchoredPosition = _spinnersOrigin[i];
        }
    }

    private IEnumerator AnimateCancelSpinners()
    {
        while (_cancelImage.fillAmount < 1)
        {
            _cancelImage.fillAmount = Mathf.Clamp01(_cancelImage.fillAmount + Time.deltaTime * _fillSpeed);
            yield return null;
        }
        _reloadSpinner.color = Color.clear;

        float t = 0;
        while (t < 0.5f)
        {
            for (int i = 0; i < _reloadCancelledSpinners.Length; i++)
            {
                var pos = _reloadCancelledSpinners[i].GetComponent<RectTransform>().anchoredPosition;
                pos += _offsets[i];
                _reloadCancelledSpinners[i].GetComponent<RectTransform>().anchoredPosition = pos;
                _reloadCancelledSpinners[i].color = new Color(1, 1, 1, (0.5f - t) * 2);
            }
            t += Time.deltaTime;
            yield return null;
        }

        foreach (var spinner in _reloadCancelledSpinners)
        {
            spinner.color = new Color(0, 0, 0, 0);
        }
    }

    private IEnumerator FadeOutCancelUI()
    {
        float t = 0;
        while (t < 0.5f)
        {
            if(_reloading) yield break;
            _cancelImage.color = new Color(1, 0, 0, (0.5f - t) * 2);
            t += Time.deltaTime;
            yield return null;
        }
        _cancelImage.color = new Color(1, 0, 0, 0);
    }

    public void ChangeWeapon(int idx)
    {
        foreach(var weapon in _weaponImages)
        {
            weapon.SetActive(false);
        }
        _weaponImages[idx].SetActive(true);

        if (idx == 0)
        {
            _ammoText.text = $"0/{_maxAmmo}";
        }
        else
        {
            _ammoText.text = $"1/1";
        }
    }
    #endregion
}
