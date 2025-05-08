using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_System : MonoBehaviour
{
    public TextMeshProUGUI StartTimerText;
    public static UI_System Instance;
    public TextMeshProUGUI StateText;
    public GameObject StartTimer;
    public Button ResumeButton;
    public Button RetryButton;
    public Button ExitButton;
    public Button CreditButton;

    private void Awake()
    {
        Instance = this;
        ResumeButton.onClick.AddListener(OnClickResumeButton);
        RetryButton.onClick.AddListener(OnClickRetryButton);
        ExitButton.onClick.AddListener(OnClickExitButton);
        CreditButton.onClick.AddListener(OnClickCreditButton);
    }
    private void Start()
    {
        StartCoroutine(ShowReadyText());
    }

    private IEnumerator ShowReadyText()
    {
        for(float i = GameManager.Instance.WaitTime; i > 0; i-=Time.unscaledDeltaTime)
        {
            StartTimerText.text = i.ToString("F2");
            yield return null;
        }
        StartTimerText.gameObject.SetActive(false);
        StateText.text = "게임 시작!";

        yield return new WaitForSeconds(1f);

        StartTimer.SetActive(false);
    }

    public void ShowGameOver()
    {
        StateText.text = "<color=red>유다희</color>";
        StartTimer.SetActive(true);
    }


    public void OnClickResumeButton()
    {
        GameManager.Instance.Resume();
    }

    public void OnClickRetryButton()
    {
        GameManager.Instance.Retry();
    }

    public void OnClickExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    private void OnClickCreditButton()
    {
        PopupManager.Instance.TryOpen(EPopupType.Popup_Credit);
    }

}
