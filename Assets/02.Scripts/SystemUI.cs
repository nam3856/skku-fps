using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class SystemUI : MonoBehaviour
{
    public TextMeshProUGUI StartTimerText;
    public static SystemUI Instance;
    public TextMeshProUGUI StateText;
    public GameObject StateCanvas;

    private void Awake()
    {
        Instance = this;
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

        StateCanvas.SetActive(false);
    }

    public void ShowGameOver()
    {
        StateText.text = "<color=red>유다희</color>";
        StateCanvas.SetActive(true);
    }
}
