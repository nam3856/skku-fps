using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup : MonoBehaviour
{
    private Action _closeCallback;

    [Header("닫기 버튼 (Optional)")]
    public Button CloseButton;

    private void Awake()
    {
        CloseButton?.onClick.AddListener(Close);
    }
    public void Open(Action closeCallback = null)
    {
        _closeCallback = closeCallback;
        gameObject.SetActive(true);
    }
    public void Close()
    {
        _closeCallback?.Invoke();
        gameObject.SetActive(false);

        if (PopupManager.Instance != null)
        {
            PopupManager.Instance.RemoveFromStack(this);
        }
    }
}
